using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services;
// Patch: refresh file context

public class EfUserService
{
    private readonly AppDbContext _db;

    public EfUserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(string username, string password, string? displayName)
    {
        var user = new User
        {
            Username = username,
            DisplayName = displayName ?? username,
            // PasswordHash will be set below using a secure hasher
        };
        // Hash password using ASP.NET Core Identity's PasswordHasher
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null) return null;
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success ? user : null;
    }

    public void SetConnection(string userId, string connectionId)
    {
        var user = _db.Users.Find(userId);
        if (user != null)
        {
            user.ConnectionId = connectionId;
            _db.SaveChanges();
        }
    }

    public void RemoveConnection(string userId)
    {
        var user = _db.Users.Find(userId);
        if (user != null)
        {
            user.ConnectionId = null;
            _db.SaveChanges();
        }
    }

    public string? GetConnection(string userId)
    {
        var user = _db.Users.Find(userId);
        return user?.ConnectionId;
    }
}
