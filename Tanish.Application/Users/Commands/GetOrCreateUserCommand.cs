using MediatR;

namespace Tanish.Application.Users.Commands;

public record GetOrCreateUserCommand(long TelegramId, string? Alias) : IRequest<Guid>;