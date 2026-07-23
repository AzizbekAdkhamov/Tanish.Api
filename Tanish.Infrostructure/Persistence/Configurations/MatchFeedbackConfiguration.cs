using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Infrastructure.Persistence.Configurations;

public class MatchFeedbackConfiguration : IEntityTypeConfiguration<MatchFeedback>
{
    public void Configure(EntityTypeBuilder<MatchFeedback> builder)
    {
        builder.HasKey(mf => new { mf.MatchId, mf.ProfileId });

        builder.HasOne(mf => mf.Match)
            .WithMany(m => m.Feedbacks)
            .HasForeignKey(mf => mf.MatchId);

        builder.HasOne(mf => mf.Profile)
            .WithMany()
            .HasForeignKey(mf => mf.ProfileId);
    }
}