using System.ComponentModel.DataAnnotations;

namespace Server.Models;

// Represents a chat group (room) for group messaging
public class Group
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Human-friendly group name
    public string Name { get; set; } = string.Empty;

    // Navigation: members of the group
    public List<GroupMember> Members { get; set; } = new();
}

public class GroupMember
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string GroupId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
