using MediatR;

namespace Tanish.Application.Matching.Commands;

public record CreateMatchCommand(List<Guid> ProfileIds) : IRequest<Guid>;