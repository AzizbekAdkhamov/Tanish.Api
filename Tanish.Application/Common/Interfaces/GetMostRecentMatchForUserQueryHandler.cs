// Tanish.Application/Matching/Queries/GetMostRecentMatchForUserQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Matching.Queries;

public class GetMostRecentMatchForUserQueryHandler : IRequestHandler<GetMostRecentMatchForUserQuery, RecentMatchDto?>
{
    private readonly IAppDbContext _db;

    public GetMostRecentMatchForUserQueryHandler(IAppDbContext db) => _db = db;

    public async Task<RecentMatchDto?> Handle(GetMostRecentMatchForUserQuery request, CancellationToken cancellationToken)
    {
        var myParticipant = await _db.MatchParticipants
            .Include(mp => mp.Profile)
            .Where(mp => mp.Profile.UserId == request.UserId)
            .OrderByDescending(mp => mp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (myParticipant is null)
            return null;

        var otherParticipant = await _db.MatchParticipants
            .Include(mp => mp.Profile)
            .ThenInclude(p => p.User)
            .Where(mp => mp.MatchId == myParticipant.MatchId && mp.ProfileId != myParticipant.ProfileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (otherParticipant is null)
            return null;

        return new RecentMatchDto(
            myParticipant.MatchId,
            myParticipant.ProfileId,
            otherParticipant.ProfileId,
            otherParticipant.Profile.User.Alias);
    }
}