using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Infrastructure.Persistence.Configurations;

internal class MatchParticipantConfiguration :IEntityTypeConfiguration<MatchParticipant>
{
    public void Configure(EntityTypeBuilder<MatchParticipant> builder)
    {
        builder.HasKey(mp => new { mp.Id, mp.ProfileId });

        builder.HasOne(mp => mp.Match)
            .WithMany(m => m.Participants)
            .HasForeignKey(mp => mp.Id);

        builder.HasOne(mp => mp.Profile)
            .WithMany()
            .HasForeignKey(mp => mp.ProfileId);
    }
}
