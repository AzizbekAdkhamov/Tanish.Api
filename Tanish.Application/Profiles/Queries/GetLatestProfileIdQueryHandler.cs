using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Profiles.Queries;

public class GetLatestProfileIdQueryHandler : IRequestHandler<GetLatestProfileIdQuery, Guid?>
{
    private readonly IAppDbContext _db;

    public GetLatestProfileIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Guid?> Handle(GetLatestProfileIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _db.ActivityProfiles
            .Where(p => p.UserId == request.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return profile?.Id;
    }
}