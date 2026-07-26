using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tanish.Application.Profiles;

namespace Tanish.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public class ActivityProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivityProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActivityProfileCommand command, CancellationToken ct)
    {
        var profileId = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(Create), new { id = profileId }, new { id = profileId });
    }
}