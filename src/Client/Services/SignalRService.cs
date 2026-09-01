using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Services;

public class SignalRService
{
    private HubConnection? _connection;
    public event System.Action<Server.Models.Message>? MessageReceived;
    public event System.Action<string, string>? PresenceUpdated; // userId, status
    public event System.Action<string, bool>? Typing; // conversationId, isTyping

    public async Task ConnectAsync(string baseUrl, string accessToken)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(new Uri(new Uri(baseUrl), "/chatHub"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken);
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<object>("MessageReceived", message =>
        {
            try
            {
                // attempt to deserialize to Message
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var msg = System.Text.Json.JsonSerializer.Deserialize<Server.Models.Message>(json);
                if (msg != null) MessageReceived?.Invoke(msg);
            }
            catch
            {
                // ignore deserialization errors
            }
        });

        _connection.On<object>("PresenceUpdated", p =>
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(p);
                var obj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
                if (obj.TryGetProperty("userId", out var u) && obj.TryGetProperty("status", out var s))
                {
                    PresenceUpdated?.Invoke(u.GetString() ?? string.Empty, s.GetString() ?? string.Empty);
                }
            }
            catch
            {
                // ignore deserialization errors
            }
        });

        _connection.On<object>("Typing", t =>
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(t);
                var obj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
                if (obj.TryGetProperty("conversationId", out var c) && obj.TryGetProperty("isTyping", out var it))
                {
                    Typing?.Invoke(c.GetString() ?? string.Empty, it.GetBoolean());
                }
            }
            catch
            {
                // ignore deserialization errors
            }
        });

        _connection.On<string>("MessageDelivered", id => { /* TODO */ });

        await _connection.StartAsync();
    }

    public async Task SendMessage(object payload)
    {
        if (_connection == null) throw new InvalidOperationException("Not connected");
        await _connection.InvokeAsync("SendMessage", payload);
    }

    public Task DisconnectAsync()
    {
        return _connection?.StopAsync() ?? Task.CompletedTask;
    }
}
