using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanish.Domain.Models;


namespace Tanish.Infrastructure.Persistence.Configurations;
public class ActivityProfileConfiguration : IEntityTypeConfiguration<ActivityProfile>
{
    public void Configure(EntityTypeBuilder<ActivityProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.BlurbText)
            .HasMaxLength(1000);

        builder.Property(p => p.BlurbEmbredding)
            .HasColumnType("vector(1536)");

        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsSearchable);
    }
}
