using Server.Models;
using System.Collections.Concurrent;

namespace Server.Services;

public class InMemoryMessageService
{
    private readonly ConcurrentDictionary<string, Message> _messages = new();

    public Message AddMessage(MessageDto dto)
    {
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = dto.SenderId,
            RecipientIds = dto.RecipientIds ?? new List<string>(),
            Text = dto.Text,
            Timestamp = DateTime.UtcNow
        };
        _messages[message.Id] = message;
        return message;
    }

    public IEnumerable<Message> GetConversation(string conversationId, int page = 0, int pageSize = 50)
    {
        return _messages.Values.Skip(page * pageSize).Take(pageSize);
    }
}
