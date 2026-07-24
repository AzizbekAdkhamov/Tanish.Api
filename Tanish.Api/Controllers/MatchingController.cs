using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tanish.Application.Matching.Commands;
using Tanish.Application.Matching.Queries;

namespace Tanish.Api.Controllers;

[ApiController]
[Route("api/matching")]
public class MatchingController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatchingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{profileId:guid}/candidates")]
    public async Task<IActionResult> GetCandidates(Guid profileId, [FromQuery] int topN = 5, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new FindMatchCandidatesQuery(profileId, topN), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateMatchCommand command, CancellationToken ct)
    {
        var matchId = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(CreateMatch), new { id = matchId }, new { id = matchId });
    }
}