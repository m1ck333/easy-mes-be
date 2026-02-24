using AlGreenMES.Modules.Orders.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Orders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PushController : ControllerBase
{
    private readonly IWebPushService _webPushService;

    public PushController(IWebPushService webPushService)
    {
        _webPushService = webPushService;
    }

    [HttpGet("vapid-public-key")]
    public IActionResult GetVapidPublicKey()
    {
        return Ok(new { publicKey = _webPushService.GetVapidPublicKey() });
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request, CancellationToken cancellationToken)
    {
        await _webPushService.SubscribeAsync(
            request.TenantId,
            request.UserId,
            request.Endpoint,
            request.P256dhKey,
            request.AuthKey,
            cancellationToken);

        return StatusCode(201);
    }

    [HttpDelete("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromQuery] string endpoint, CancellationToken cancellationToken)
    {
        await _webPushService.UnsubscribeAsync(endpoint, cancellationToken);
        return NoContent();
    }
}

public record SubscribeRequest(
    Guid TenantId,
    Guid UserId,
    string Endpoint,
    string P256dhKey,
    string AuthKey);
