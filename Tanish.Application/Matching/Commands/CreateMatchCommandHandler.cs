using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Enums;
using Tanish.Domain.Models.MatchModels;
using Tanish.Domain.Rules;

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

        var distinctCategories = profiles.Select(p => p.Category).Distinct().ToList();
        if (distinctCategories.Count > 1)
            throw new InvalidOperationException("All participants in a match must share the same activity category.");

        var category = distinctCategories[0];
        var (min, max) = ActivityGroupSizeRules.GetRange(category);

        if (profiles.Count < min || profiles.Count > max)
            throw new InvalidOperationException($"For {category}, a match must have between {min} and {max} participants.");

        var match = new Match
        {
            Status = MatchStatus.Proposed,
            Participants = profiles.Select(p => new MatchParticipant
            {
                ProfileId = p.Id
            }).ToList()
        };

        _db.Matches.Add(match);

        foreach (var profile in profiles)
        {
            profile.IsSearchable = false;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return match.Id;
    }
}