// Tanish.Application/Matching/Commands/CreateReportCommand.cs
using MediatR;

namespace Tanish.Application.Matching.Commands;

public record CreateReportCommand(Guid MatchId, Guid ReporterProfileId, Guid ReportedProfileId, string Reason) : IRequest<Guid>;