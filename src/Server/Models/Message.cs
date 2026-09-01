namespace Server.Models;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SenderId { get; set; } = string.Empty;
    // Comma-separated recipient ids for simple storage; in production use a separate table
    public List<string> RecipientIds { get; set; } = new();

    // Text content (null for media messages)
    public string? Text { get; set; }

    // Optional attachment URL(s)
    public List<string>? Attachments { get; set; }

    // Message creation time
    public DateTime Timestamp { get; set; }
}
