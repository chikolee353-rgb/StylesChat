using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Data;
using Server.Models;

namespace Server.Hubs;

[Authorize]
public sealed class ChatHub(ChatDbContext database) : Hub
{
    public async Task SendMessage(MessageDto message)
    {
        var text = message.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text) || text.Length > 2000)
        {
            throw new HubException("Messages must contain 1–2000 characters.");
        }

        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = Context.User?.Identity?.Name;
        if (!int.TryParse(userId, out var parsedUserId) || string.IsNullOrWhiteSpace(username))
        {
            throw new HubException("The authenticated user is invalid.");
        }

        var savedMessage = new Message
        {
            SenderId = username,
            Text = text,
            Timestamp = DateTime.UtcNow
        };
        database.Messages.Add(savedMessage);
        await database.SaveChangesAsync();
        await Clients.All.SendAsync("MessageReceived", savedMessage);
    }
}
