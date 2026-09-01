using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services;

public class EfMessageService
{
    private readonly AppDbContext _db;

    public EfMessageService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Message> AddMessageAsync(MessageDto dto)
    {
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = dto.SenderId,
            RecipientIds = dto.RecipientIds ?? new List<string>(),
            Text = dto.Text,
            Timestamp = DateTime.UtcNow
        };
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(string conversationId, int page = 0, int pageSize = 50)
    {
        // Very simple: return recent messages. Conversation grouping not implemented in MVP.
        return await _db.Messages.OrderByDescending(m => m.Timestamp).Skip(page * pageSize).Take(pageSize).ToListAsync();
    }
}
