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
        await DisconnectAsync();
        _connection = new HubConnectionBuilder()
            .WithUrl(new Uri(new Uri(baseUrl), "/chatHub"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<Server.Models.Message>("MessageReceived", message => MessageReceived?.Invoke(message));

        await _connection.StartAsync();
    }

    public async Task SendMessage(object payload)
    {
        if (_connection == null) throw new InvalidOperationException("Not connected");
        await _connection.InvokeAsync("SendMessage", payload);
    }

    public async Task DisconnectAsync()
    {
        var connection = _connection;
        _connection = null;
        if (connection is null) return;
        await connection.StopAsync();
        await connection.DisposeAsync();
    }
}
