using Tanish.Domain.Commons;
using Tanish.Domain.Models.Profile;

namespace Tanish.Domain.Models.MatchModels;
public class MatchParticipant: BaseEntity
{
    public Match Match { get; set; }
    public Guid MatchId { get; set; }
    public Guid ProfileId { get; set; }
    public ActivityProfile Profile { get; set; }
}
