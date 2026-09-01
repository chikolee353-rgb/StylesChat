using Server.Models;
using System.Collections.Concurrent;

namespace Server.Services;

public class InMemoryUserService
{
    private readonly ConcurrentDictionary<string, User> _usersById = new();
    private readonly ConcurrentDictionary<string, string> _usersByUsername = new();

    public User? GetByUsername(string username)
    {
        if (_usersByUsername.TryGetValue(username.ToLowerInvariant(), out var id))
        {
            if (_usersById.TryGetValue(id, out var user)) return user;
        }
        return null;
    }

    public User? GetById(string id)
    {
        _usersById.TryGetValue(id, out var user);
        return user;
    }

    public User CreateUser(string username, string password, string? displayName)
    {
        var user = new User
        {
            Username = username,
            DisplayName = displayName ?? username,
            PasswordHash = password // WARNING: store hashed in production
        };
        _usersById[user.Id] = user;
        _usersByUsername[username.ToLowerInvariant()] = user.Id;
        return user;
    }

    public User? ValidateCredentials(string username, string password)
    {
        var user = GetByUsername(username);
        if (user == null) return null;
        if (user.PasswordHash == password) return user;
        return null;
    }

    public void SetConnection(string userId, string connectionId)
    {
        if (_usersById.TryGetValue(userId, out var user))
        {
            user.ConnectionId = connectionId;
        }
    }

    public void RemoveConnection(string userId)
    {
        if (_usersById.TryGetValue(userId, out var user))
        {
            user.ConnectionId = null;
        }
    }

    public string? GetConnection(string userId)
    {
        if (_usersById.TryGetValue(userId, out var user))
            return user.ConnectionId;
        return null;
    }
}
