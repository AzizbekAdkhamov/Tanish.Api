using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Enums;

namespace Tanish.Application.Matching.Jobs;

public class CleanupStaleMatchesJob
{
    private readonly IAppDbContext _db;

    public CleanupStaleMatchesJob(IAppDbContext db) => _db = db;

    public async Task RunAsync(CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);

        var staleMatches = await _db.Matches
            .Where(m => m.Status == MatchStatus.Proposed && m.CreatedAt < cutoff)
            .Include(m => m.Participants)
            .ToListAsync(ct);

        foreach (var match in staleMatches)
        {
            match.Status = MatchStatus.Ended;

            var profileIds = match.Participants.Select(p => p.ProfileId).ToList();
            var profiles = await _db.ActivityProfiles
                .Where(p => profileIds.Contains(p.Id))
                .ToListAsync(ct);

            foreach (var profile in profiles)
                profile.IsSearchable = true; // put them back in the searchable pool
        }

        await _db.SaveChangesAsync(ct);
    }
}