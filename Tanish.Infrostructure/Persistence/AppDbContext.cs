// Tanish.Infrostructure/Persistence/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Models.MatchModels;
using Tanish.Domain.Models.Profile;
using Tanish.Domain.Models.User;

namespace Tanish.Persistence.DbContexts;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<ActivityProfile> ActivityProfiles => Set<ActivityProfile>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchFeedback> MatchFeedbacks => Set<MatchFeedback>();
    public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();
    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}