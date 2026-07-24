using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Matching.Queries;

public class GetProfileOwnerTelegramIdQueryHandler : IRequestHandler<GetProfileOwnerTelegramIdQuery, long?>
{
    private readonly IAppDbContext _db;

    public GetProfileOwnerTelegramIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<long?> Handle(GetProfileOwnerTelegramIdQuery request, CancellationToken cancellationToken)
    {
        var telegramId = await _db.ActivityProfiles
            .Where(p => p.Id == request.ProfileId)
            .Select(p => p.User.TelegramId)
            .FirstOrDefaultAsync(cancellationToken);

        return telegramId == 0 ? null : telegramId;
    }
}