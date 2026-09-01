namespace Server.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    // Hashed password (use secure hashing in production)
    public string PasswordHash { get; set; } = string.Empty;

    // SignalR connection id (transient)
    public string? ConnectionId { get; set; }

    // Navigation: groups the user belongs to
    public List<GroupMember>? GroupMemberships { get; set; }
}
