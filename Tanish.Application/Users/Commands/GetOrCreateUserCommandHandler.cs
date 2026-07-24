using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Models.User;

namespace Tanish.Application.Users.Commands;

public class GetOrCreateUserCommandHandler : IRequestHandler<GetOrCreateUserCommand, Guid>
{
    private readonly IAppDbContext _db;

    public GetOrCreateUserCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(GetOrCreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.Users
            .FirstOrDefaultAsync(u => u.TelegramId == request.TelegramId, cancellationToken);

        if (existing is not null)
        {
            // keep alias fresh if it changed on Telegram's side
            if (!string.IsNullOrWhiteSpace(request.Alias) && existing.Alias != request.Alias)
            {
                existing.Alias = request.Alias;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return existing.Id;
        }

        var user = new AppUser
        {
            TelegramId = request.TelegramId,
            Alias = request.Alias ?? $"user_{request.TelegramId}",
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}