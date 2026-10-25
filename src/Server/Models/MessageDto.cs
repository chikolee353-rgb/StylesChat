namespace Server.Models;

public sealed class MessageDto
{
    public string? SenderId { get; set; }
    public List<string>? RecipientIds { get; set; }
    public string? Text { get; set; }
}
