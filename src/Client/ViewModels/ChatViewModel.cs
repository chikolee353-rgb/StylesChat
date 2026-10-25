using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows.Input;
using Microsoft.Maui.Dispatching;
using Client.Services;
using Server.Models;

namespace Client.ViewModels;

// ViewModel for ChatPage. Handles SignalR connection and exposes messages to the UI.
public class ChatViewModel : BindableObject
{
    private readonly SignalRService _signalR = new SignalRService();

    public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

    private string _outgoingText = string.Empty;
    public string OutgoingText
    {
        get => _outgoingText;
        set { _outgoingText = value; OnPropertyChanged(); }
    }

    public ICommand SendCommand { get; }
    private bool _initialized;

    public ChatViewModel()
    {
        SendCommand = new Command(async () => await SendAsync());
        _signalR.MessageReceived += OnMessageReceived;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        var token = Preferences.Get("accessToken", string.Empty);
        if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Please sign in again.");

        using var client = CreateAuthenticatedClient(token);
        var history = await client.GetFromJsonAsync<List<Message>>("api/messages");
        Messages.Clear();
        foreach (var message in history ?? [])
        {
            Messages.Add(message);
        }

        await _signalR.ConnectAsync(ApiConfiguration.BaseUrl, token);
        _initialized = true;
    }

    private void OnMessageReceived(Server.Models.Message msg)
    {
        MainThread.BeginInvokeOnMainThread(() => Messages.Insert(0, msg));
    }

    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(OutgoingText)) return;
        var dto = new MessageDto { Text = OutgoingText };
        await _signalR.SendMessage(dto);
        OutgoingText = string.Empty;
    }

    private static HttpClient CreateAuthenticatedClient(string token)
    {
        var client = new HttpClient { BaseAddress = new Uri(ApiConfiguration.BaseUrl) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
