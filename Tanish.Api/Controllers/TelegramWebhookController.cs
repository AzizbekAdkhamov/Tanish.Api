using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Tanish.Api.TelegramBot;

namespace Tanish.Api.Controllers;

[ApiController]
[Route("api/telegram/webhook")]
public class TelegramWebhookController : ControllerBase
{
    private readonly TelegramUpdateHandler _handler;

    public TelegramWebhookController(TelegramUpdateHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, CancellationToken ct)
    {
        await _handler.HandleUpdateAsync(update, ct);
        return Ok();
    }
}