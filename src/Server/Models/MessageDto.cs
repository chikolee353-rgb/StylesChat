namespace Server.Models;

public class MessageDto
{
    public string SenderId { get; set; } = string.Empty;
    public List<string>? RecipientIds { get; set; }
    public string? Text { get; set; }
}
