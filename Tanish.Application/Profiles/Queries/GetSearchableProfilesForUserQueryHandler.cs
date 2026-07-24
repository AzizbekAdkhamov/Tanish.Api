// Tanish.Application/Profiles/Queries/GetSearchableProfilesForUserQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Profiles.Queries;

public class GetSearchableProfilesForUserQueryHandler : IRequestHandler<GetSearchableProfilesForUserQuery, List<ProfileSummaryDto>>
{
    private readonly IAppDbContext _db;

    public GetSearchableProfilesForUserQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<ProfileSummaryDto>> Handle(GetSearchableProfilesForUserQuery request, CancellationToken cancellationToken)
    {
        return await _db.ActivityProfiles
            .Where(p => p.UserId == request.UserId && p.IsSearchable)
            .Select(p => new ProfileSummaryDto(p.Id, p.Category))
            .ToListAsync(cancellationToken);
    }
}