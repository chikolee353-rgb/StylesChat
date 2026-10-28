var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddSignalR();
var app = builder.Build();
app.UseCors("AllowAll");

app.MapGet("/", () => Results.Ok(new { message = "StylesChat API LIVE", status = "Online", backend = "styleschat-chikolee353-rgb", db = "styleschat-db", time = DateTime.Now }));
app.MapGet("/healthz", () => Results.Ok("Healthy"));

app.MapGet("/api/users", () => new[] {
    new { id = "1", name = "Mbare Creator", avatar = "MC", online = true },
    new { id = "2", name = "Masvingo Star", avatar = "MS", online = true },
    new { id = "3", name = "Leeroy Chikondo", avatar = "LC", online = true }
});

var messages = new System.Collections.Concurrent.ConcurrentBag<object>();
app.MapGet("/api/messages/{chatId}", (string chatId) => messages);
app.MapPost("/api/messages", (dynamic msg) => { messages.Add(msg); return Results.Ok(new { success = true, data = msg }); });
app.MapHub<ChatHub>("/chatHub");
app.Run();
class ChatHub : Microsoft.AspNetCore.SignalR.Hub {
    public async Task SendMessage(string user, string message) => await Clients.All.SendAsync("ReceiveMessage", user, message);
}
