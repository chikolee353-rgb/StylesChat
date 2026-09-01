using System.Threading.Tasks;

namespace Client.Services;

public class CallService
{
    // Placeholder: integrate an SDK here (Agora, Twilio) or use a WebView-based WebRTC page.
    // Responsibilities:
    // - Request microphone/camera permissions
    // - Start/stop local preview
    // - Create/answer calls using signaling (SignalR)
    // - Expose events for remote stream availability

    public Task InitializeAsync() => Task.CompletedTask;

    public Task StartLocalPreviewAsync() => Task.CompletedTask;

    public Task StopLocalPreviewAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}
