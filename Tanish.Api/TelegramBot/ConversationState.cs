using Tanish.Domain.Enums;

namespace Tanish.Api.TelegramBot
{
    public class ConversationState
    {
        public ConversationStep Step { get; set; } = ConversationStep.None;
        public ActivityCategory? Category { get; set; }
        public ExperienceLevel? Level { get; set; }
        public string? Availability { get; set; }
        public string? BlurbText { get; set; }
        public Guid? ActiveProfileId { get; set; }
        public List<Guid> PendingCandidateIds { get; set; } = new();
        public Guid? PendingReportMatchId { get; set; }
        public Guid? PendingReporterProfileId { get; set; }
        public Guid? PendingReportedProfileId { get; set; }
    }
}
