using Tanish.Domain.Commons;
using Tanish.Domain.Enums;

namespace Tanish.Domain.Models.MatchModels;

public class Match: BaseEntity
{
    public MatchStatus Status { get; set; }
    public ICollection<MatchParticipant> Participants { get; set; } = new List<MatchParticipant>();
    public ICollection<MatchFeedback> Feedbacks { get; set; } = new List<MatchFeedback>();
}
