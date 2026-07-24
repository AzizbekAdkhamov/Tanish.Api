// Tanish.Application/Matching/Queries/GetMostRecentMatchForUserQuery.cs
using MediatR;

namespace Tanish.Application.Matching.Queries;

public record GetMostRecentMatchForUserQuery(Guid UserId) : IRequest<RecentMatchDto?>;

public record RecentMatchDto(Guid MatchId, Guid ReporterProfileId, Guid ReportedProfileId, string ReportedAlias);