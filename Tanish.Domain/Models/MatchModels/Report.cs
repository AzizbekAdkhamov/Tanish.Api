using Tanish.Domain.Commons;

namespace Tanish.Domain.Models.MatchModels;
public class Report : BaseEntity
{
    public Guid MatchId { get; set; }
    public Match Match { get; set; }
    public Guid ReporterProfileId { get; set; }
    public ActivityProfile ReporterProfile { get; set; }  

    public Guid ReportedProfileId { get; set; }
    public ActivityProfile ReportedProfile { get; set; }
    public string Reason { get; set; }
}
