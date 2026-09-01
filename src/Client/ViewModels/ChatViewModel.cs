using System.Collections.ObjectModel;
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

    public ChatViewModel()
    {
        SendCommand = new Command(async () => await SendAsync());
        _signalR.MessageReceived += OnMessageReceived;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        // Use local dev server base url; in production use configuration
        var token = Preferences.Get("accessToken", string.Empty);
        if (string.IsNullOrEmpty(token)) return;
        await _signalR.ConnectAsync("https://10.0.2.2:5001", token); // emulator loopback mapping
    }

    private void OnMessageReceived(Server.Models.Message msg)
    {
        MainThread.BeginInvokeOnMainThread(() => Messages.Insert(0, msg));
    }

    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(OutgoingText)) return;
        var dto = new MessageDto { SenderId = "", RecipientIds = new List<string> { /* recipient id */ }, Text = OutgoingText };
        await _signalR.SendMessage(dto);
        OutgoingText = string.Empty;
    }
}
