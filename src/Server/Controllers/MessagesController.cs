using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly Server.Services.EfMessageService _messages;

    public MessagesController(Server.Services.EfMessageService messages)
    {
        _messages = messages;
    }

    [HttpGet("conversation/{id}")]
    public async Task<IActionResult> GetConversation(string id, int page = 0, int pageSize = 50)
    {
        var items = await _messages.GetConversationAsync(id, page, pageSize);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> PostMessage([FromBody] MessageDto dto)
    {
        var message = await _messages.AddMessageAsync(dto);
        // Broadcast via SignalR would happen in hub; controllers persist messages if necessary
        return CreatedAtAction(null, new { id = message.Id }, message);
    }
}
