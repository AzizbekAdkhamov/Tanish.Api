using MediatR;
using Tanish.Domain.Enums;

namespace Tanish.Application.Profiles.Commands;

public record CreateActivityProfileCommand(
    Guid UserId,
    ActivityCategory Category,
    ExperienceLevel Level,
    string Availability,
    string BlurbText
) : IRequest<Guid>;