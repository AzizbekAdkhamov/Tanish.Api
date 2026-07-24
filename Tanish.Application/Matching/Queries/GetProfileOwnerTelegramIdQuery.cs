using MediatR;

namespace Tanish.Application.Matching.Queries;

public record GetProfileOwnerTelegramIdQuery(Guid ProfileId) : IRequest<long?>;