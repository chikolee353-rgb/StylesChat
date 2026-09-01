using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Services;
using Microsoft.Extensions.Configuration;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly EfUserService _users;
    private readonly IConfiguration _config;

    public AuthController(EfUserService users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (await _users.GetByUsernameAsync(req.Username) != null)
            return Conflict(new { message = "Username already taken" });
        var user = await _users.CreateUserAsync(req.Username, req.Password, req.DisplayName);
        return CreatedAtAction(nameof(Me), new { id = user.Id }, new { id = user.Id, username = user.Username });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _users.ValidateCredentialsAsync(req.Username, req.Password);
        if (user == null) return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateJwtToken(user);
        return Ok(new { accessToken = token, expiresIn = 3600, user = new { user.Id, user.Username, user.DisplayName } });
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null) return Unauthorized();
        var user = await _users.GetByIdAsync(id);
        return Ok(new { user.Id, user.Username, user.DisplayName });
    }

    private string GenerateJwtToken(User user)
    {
        // No-op patch: ensure file saved correctly.
        var key = _config["Jwt:Key"] ?? "super_secret_dev_key_change_me";
        var issuer = _config["Jwt:Issuer"] ?? "masvegas.chat";
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("displayName", user.DisplayName ?? string.Empty)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
