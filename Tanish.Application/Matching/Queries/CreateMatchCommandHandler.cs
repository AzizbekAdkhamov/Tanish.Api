using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Enums;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Application.Matching.Commands;

public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateMatchCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        if (request.ProfileIds.Count < 2)
            throw new InvalidOperationException("A match needs at least 2 participants.");

        var profiles = await _db.ActivityProfiles
            .Where(p => request.ProfileIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (profiles.Count != request.ProfileIds.Count)
            throw new KeyNotFoundException("One or more profiles were not found.");

        var match = new Match
        {
            Status = MatchStatus.Proposed,
            Participants = profiles.Select(p => new MatchParticipant
            {
                ProfileId = p.Id
            }).ToList()
        };

        _db.Matches.Add(match);

        // Once matched, take these profiles out of the searchable pool
        foreach (var profile in profiles)
        {
            profile.IsSearchable = false;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return match.Id;
    }
}