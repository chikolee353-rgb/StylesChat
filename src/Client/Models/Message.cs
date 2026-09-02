using System;
using System.Collections.Generic;

namespace Server.Models;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? SenderId { get; set; }
    public List<string> RecipientIds { get; set; } = new();
    public string? Text { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Attachments { get; set; }
}
