// Tanish.Application/Profiles/Commands/StopSearchingCommandHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Profiles.Commands;

public class StopSearchingCommandHandler : IRequestHandler<StopSearchingCommand, int>
{
    private readonly IAppDbContext _db;

    public StopSearchingCommandHandler(IAppDbContext db) => _db = db;

    public async Task<int> Handle(StopSearchingCommand request, CancellationToken cancellationToken)
    {
        var profiles = await _db.ActivityProfiles
            .Where(p => p.UserId == request.UserId && p.IsSearchable)
            .ToListAsync(cancellationToken);

        foreach (var profile in profiles)
            profile.IsSearchable = false;

        await _db.SaveChangesAsync(cancellationToken);
        return profiles.Count;
    }
}