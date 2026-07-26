using MediatR;
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Application.Users.Commands;

public class SetProfilePhotoCommandHandler : IRequestHandler<SetProfilePhotoCommand>
{
    private readonly IAppDbContext _db;

    public SetProfilePhotoCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(SetProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.TelegramId == request.TelegramId, cancellationToken);

        if (user is null)
            return;

        user.TelegramPhotoFileId = request.PhotoFileId;
        await _db.SaveChangesAsync(cancellationToken);
    }
}