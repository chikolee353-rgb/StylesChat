using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Hubs;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);
var signingKey = builder.Configuration["JWT_SIGNING_KEY"];
if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
{
    throw new InvalidOperationException("Set JWT_SIGNING_KEY to a secret of at least 32 bytes.");
}

var databaseUrl = builder.Configuration["DATABASE_URL"];
if (string.IsNullOrWhiteSpace(databaseUrl))
{
    var databasePath = builder.Configuration["CHAT_DATABASE_PATH"] ?? "styleschat.db";
    var databaseDirectory = Path.GetDirectoryName(databasePath);
    if (!string.IsNullOrEmpty(databaseDirectory))
    {
        Directory.CreateDirectory(databaseDirectory);
    }

    builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlite($"Data Source={databasePath}"));
}
else
{
    var databaseUri = new Uri(databaseUrl);
    var databaseConnection = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
        Username = Uri.UnescapeDataString(databaseUri.UserInfo.Split(':', 2)[0]),
        Password = Uri.UnescapeDataString(databaseUri.UserInfo.Split(':', 2)[1]),
        Database = Uri.UnescapeDataString(databaseUri.AbsolutePath.TrimStart('/')),
        SslMode = Npgsql.SslMode.Require
    };
    builder.Services.AddDbContext<ChatDbContext>(options => options.UseNpgsql(databaseConnection.ConnectionString));
}

builder.Services.AddSingleton<IPasswordHasher<ChatUser>, PasswordHasher<ChatUser>>();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "StylesChat",
            ValidateAudience = true,
            ValidAudience = "StylesChat",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/chatHub"))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    await database.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/auth/register", async (
    RegisterRequest request,
    ChatDbContext database,
    IPasswordHasher<ChatUser> passwordHasher) =>
{
    var username = request.Username?.Trim();
    if (string.IsNullOrWhiteSpace(username) || username.Length is < 3 or > 32 ||
        username.Any(character => !char.IsLetterOrDigit(character) && character != '_'))
    {
        return Results.BadRequest(new { error = "Username must be 3–32 letters, numbers, or underscores." });
    }

    if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8 || request.Password.Length > 128)
    {
        return Results.BadRequest(new { error = "Password must be 8–128 characters." });
    }

    var normalizedUsername = username.ToUpperInvariant();
    if (await database.Users.AnyAsync(user => user.NormalizedUsername == normalizedUsername))
    {
        return Results.Conflict(new { error = "That username is already registered." });
    }

    var user = new ChatUser { Username = username, NormalizedUsername = normalizedUsername };
    user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
    database.Users.Add(user);
    await database.SaveChangesAsync();
    return Results.Ok(new AuthResponse(CreateToken(user, signingKey)));
});

app.MapPost("/api/auth/login", async (
    LoginRequest request,
    ChatDbContext database,
    IPasswordHasher<ChatUser> passwordHasher) =>
{
    var username = request.Username?.Trim();
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(request.Password))
    {
        return Results.BadRequest(new { error = "Username and password are required." });
    }

    var normalizedUsername = username.ToUpperInvariant();
    var user = await database.Users.SingleOrDefaultAsync(
        existing => existing.NormalizedUsername == normalizedUsername);
    if (user is null ||
        passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new AuthResponse(CreateToken(user, signingKey)));
});

app.MapGet("/api/messages", async (ChatDbContext database) =>
    Results.Ok(await database.Messages
        .OrderByDescending(message => message.Timestamp)
        .Take(100)
        .ToListAsync()))
    .RequireAuthorization();

app.MapHub<ChatHub>("/chatHub").RequireAuthorization();
app.Run();

static string CreateToken(ChatUser user, string signingKey)
{
    var credentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: "StylesChat",
        audience: "StylesChat",
        claims:
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        ],
        expires: DateTime.UtcNow.AddHours(12),
        signingCredentials: credentials);
    return new JwtSecurityTokenHandler().WriteToken(token);
}

record RegisterRequest(string? Username, string? Password);
record LoginRequest(string? Username, string? Password);
record AuthResponse(string AccessToken);
