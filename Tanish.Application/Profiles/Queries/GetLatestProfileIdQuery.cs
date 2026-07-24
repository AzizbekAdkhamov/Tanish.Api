using MediatR;

namespace Tanish.Application.Profiles.Queries;

public record GetLatestProfileIdQuery(Guid UserId) : IRequest<Guid?>;