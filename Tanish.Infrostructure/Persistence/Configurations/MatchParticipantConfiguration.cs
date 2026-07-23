using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Infrastructure.Persistence.Configurations;

public class MatchParticipantConfiguration : IEntityTypeConfiguration<MatchParticipant>
{
    public void Configure(EntityTypeBuilder<MatchParticipant> builder)
    {
        builder.HasKey(mp => new { mp.MatchId, mp.ProfileId });

        builder.HasOne(mp => mp.Match)
            .WithMany(m => m.Participants)
            .HasForeignKey(mp => mp.MatchId);

        builder.HasOne(mp => mp.Profile)
            .WithMany()
            .HasForeignKey(mp => mp.ProfileId);
    }
}