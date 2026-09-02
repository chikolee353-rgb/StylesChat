using System.Collections.Generic;

namespace Server.Models;

public class MessageDto
{
    public string? SenderId { get; set; }
    public List<string>? RecipientIds { get; set; }
    public string? Text { get; set; }
}
