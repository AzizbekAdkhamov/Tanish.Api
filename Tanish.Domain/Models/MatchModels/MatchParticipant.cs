using Tanish.Domain.Commons;

namespace Tanish.Domain.Models.MatchModels;
public class MatchParticipant: BaseEntity
{
    public Match Match { get; set; }
    public Guid ProfileId { get; set; }
    public ActivityProfile Profile { get; set; }
}
