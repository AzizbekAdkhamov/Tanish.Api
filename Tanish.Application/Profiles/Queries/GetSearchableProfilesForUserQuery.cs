// Tanish.Application/Profiles/Queries/GetSearchableProfilesForUserQuery.cs
using MediatR;
using Tanish.Domain.Enums;

namespace Tanish.Application.Profiles.Queries;

public record GetSearchableProfilesForUserQuery(Guid UserId) : IRequest<List<ProfileSummaryDto>>;

public record ProfileSummaryDto(Guid ProfileId, ActivityCategory Category);