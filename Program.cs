using Microsoft.AspNetCore.SignalR;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddSignalR();
var app = builder.Build();
app.UseCors("AllowAll");
app.MapGet("/", () => new { message = "StylesChat API LIVE", status = "Online", version = "v2-fixed", time = DateTime.UtcNow });
app.MapGet("/healthz", () => "Healthy");
app.MapGet("/api/users", () => new[] {
    new { id="1", name="Mbare Creator", avatar="MC", online=true },
    new { id="2", name="Masvingo Star", avatar="MS", online=true },
    new { id="3", name="Leeroy Chikondo", avatar="LC", online=true }
});
app.MapHub<ChatHub>("/chatHub");
app.Run();
class ChatHub : Microsoft.AspNetCore.SignalR.Hub {
    public async Task SendMessage(string user, string message) => await Clients.All.SendAsync("ReceiveMessage", user, message);
}
