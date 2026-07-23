using Microsoft.EntityFrameworkCore;
using Tanish.Domain.Models;
using Tanish.Domain.Models.MatchModels;

namespace Tanish.Persistence.DbContexts;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<ActivityProfile> ActivityProfiles => Set<ActivityProfile>();
    public DbSet<Match> matches => Set<Match>();
    public DbSet<MatchFeedback> matchesFeedback => Set<MatchFeedback>();
    public DbSet<MatchParticipant> matchesParticipant => Set<MatchParticipant>();
    public DbSet<Report> reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
