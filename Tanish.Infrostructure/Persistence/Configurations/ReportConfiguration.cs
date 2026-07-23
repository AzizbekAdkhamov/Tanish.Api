using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Infrastructure.Persistence.Configurations;
public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasOne(r => r.ReporterProfile)
        .WithMany()
        .HasForeignKey(r => r.ReporterProfileId)
        .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete reports if a profile is deleted, keep the record

        builder.HasOne(r => r.ReportedProfile)
            .WithMany()
            .HasForeignKey(r => r.ReportedProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
