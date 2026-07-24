// Tanish.Application/Matching/Commands/CreateReportCommandHandler.cs
using MediatR;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Application.Matching.Commands;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateReportCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            MatchId = request.MatchId,
            ReporterProfileId = request.ReporterProfileId,
            ReportedProfileId = request.ReportedProfileId,
            Reason = request.Reason
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync(cancellationToken);

        return report.Id;
    }
}