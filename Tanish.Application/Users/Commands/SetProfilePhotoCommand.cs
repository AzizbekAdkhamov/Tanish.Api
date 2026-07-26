using MediatR;

namespace Tanish.Application.Users.Commands;

public record SetProfilePhotoCommand(long TelegramId, string PhotoFileId) : IRequest;