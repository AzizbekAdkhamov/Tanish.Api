using Tanish.Domain.Commons;

namespace Tanish.Domain.Models.MatchModels;
public class MatchFeedback :BaseEntity
{
    public Guid MatchId { get; set; }
    public Match Match { get; set; }
    public Guid ProfileId { get; set; }
    public ActivityProfile Profile { get; set; }
    public bool WorkedOut { get; set; }
    public string? Note {  get; set; }
}
