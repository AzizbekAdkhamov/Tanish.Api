using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanish.Domain.Models;

namespace Tanish.Infrastructure.Persistence.Configurationsl;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
       builder.HasKey(x => x.Id);

       builder.Property(u => u.TelegramId)
            .IsRequired();
       builder.HasIndex(u => u.TelegramId)
            .IsUnique();
       builder.Property(u => u.Alias)
             .HasMaxLength(50);
       builder.HasMany(u => u.Profiles)
             .WithOne(p => p.User)
             .HasForeignKey(p =>p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
    }
}
