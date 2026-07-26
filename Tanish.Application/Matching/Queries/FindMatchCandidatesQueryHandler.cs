using MediatR;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Matching.Queries;

public class FindMatchCandidatesQueryHandler : IRequestHandler<FindMatchCandidatesQuery, List<MatchCandidateDto>>
{
    private readonly IAppDbContext _db;

    public FindMatchCandidatesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<MatchCandidateDto>> Handle(FindMatchCandidatesQuery request, CancellationToken cancellationToken)
    {
        var profile = await _db.ActivityProfiles
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId, cancellationToken)
            ?? throw new KeyNotFoundException("Profile not found");

        var candidates = await _db.ActivityProfiles
            .Where(p => p.Id != profile.Id
                && p.IsSearchable
                && p.Category == profile.Category
                && p.Level == profile.Level
                && p.Availability == profile.Availability)
            .Include(p => p.User)
            .OrderBy(p => p.BlurbEmbedding.CosineDistance(profile.BlurbEmbedding))
            .Take(request.TopN)
            .Select(p => new MatchCandidateDto(
                p.Id,
                p.User.Alias,
                p.Category,
                p.Level,
                p.Availability,
                p.User.TelegramPhotoFileId,
                p.BlurbEmbedding.CosineDistance(profile.BlurbEmbedding)))
            .ToListAsync(cancellationToken);

        return candidates;
    }
}