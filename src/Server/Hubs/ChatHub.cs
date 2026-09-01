using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using Server.Services;

namespace Server.Hubs;

[Authorize]
// ChatHub handles real-time messaging, presence and typing indicators.
public class ChatHub : Hub
{
    private readonly EfUserService _users;
    private readonly EfMessageService _messages;

    public ChatHub(EfUserService users, EfMessageService messages)
    {
        _users = users;
        _messages = messages;
    }

    // Called when client connects. Store connection id for presence.
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _users.SetConnection(userId, Context.ConnectionId);
            // Notify others that this user is online
            await Clients.Others.SendAsync("PresenceUpdated", new { userId, status = "online" });
        }
        await base.OnConnectedAsync();
    }

    // Called when client disconnects. Clear connection id and broadcast presence
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _users.RemoveConnection(userId);
            await Clients.Others.SendAsync("PresenceUpdated", new { userId, status = "offline" });
        }
        await base.OnDisconnectedAsync(exception);
    }

    // Send a direct message to one or more recipients
    public async Task SendMessage(MessageDto payload)
    {
        // Persist message
        var message = await _messages.AddMessageAsync(payload);

        // Deliver to recipients if online
        foreach (var recipientId in message.RecipientIds)
        {
            var conn = _users.GetConnection(recipientId);
            if (!string.IsNullOrEmpty(conn))
            {
                await Clients.Client(conn).SendAsync("MessageReceived", message);
            }
        }

        // Acknowledge to sender
        await Clients.Caller.SendAsync("MessageDelivered", message.Id);
    }

    // Typing indicator: notify other members in the conversation
    public async Task Typing(string conversationId, bool isTyping)
    {
        await Clients.OthersInGroup(conversationId).SendAsync("Typing", new { conversationId, userId = Context.UserIdentifier, isTyping });
    }

    // Create or join a group (conversation)
    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        await Clients.Group(groupId).SendAsync("GroupMemberJoined", new { groupId, userId = Context.UserIdentifier });
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        await Clients.Group(groupId).SendAsync("GroupMemberLeft", new { groupId, userId = Context.UserIdentifier });
    }

    // Send a message to a group
    public async Task SendGroupMessage(string groupId, MessageDto payload)
    {
        var message = await _messages.AddMessageAsync(payload);
        await Clients.Group(groupId).SendAsync("GroupMessageReceived", message);
        await Clients.Caller.SendAsync("MessageDelivered", message.Id);
    }

    // Relay WebRTC signaling messages for call setup
    public async Task CallSignal(string targetUserId, object data)
    {
        var conn = _users.GetConnection(targetUserId);
        if (!string.IsNullOrEmpty(conn))
        {
            await Clients.Client(conn).SendAsync("CallSignal", Context.UserIdentifier, data);
        }
    }

    // Start a call by notifying the target user
    public async Task StartCall(string targetUserId)
    {
        var conn = _users.GetConnection(targetUserId);
        if (!string.IsNullOrEmpty(conn))
        {
            await Clients.Client(conn).SendAsync("IncomingCall", new { From = Context.UserIdentifier });
        }
    }

    // End call notification
    public async Task EndCall(string targetUserId)
    {
        var conn = _users.GetConnection(targetUserId);
        if (!string.IsNullOrEmpty(conn))
        {
            await Clients.Client(conn).SendAsync("CallEnded", new { From = Context.UserIdentifier });
        }
    }
}
