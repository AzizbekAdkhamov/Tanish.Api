// Tanish.Application/Profiles/Commands/StopSearchingCommand.cs
using MediatR;

namespace Tanish.Application.Profiles;

public record StopSearchingCommand(Guid UserId) : IRequest<int>; // returns count of profiles stopped