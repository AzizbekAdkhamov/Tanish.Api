using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Tanish.Domain.Models.MatchModels;
using Tanish.Domain.Models.Profile;
using Tanish.Domain.Models.User;

namespace Tanish.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<AppUser> Users { get; }
    DbSet<ActivityProfile> ActivityProfiles { get; }
    DbSet<Match> Matches { get; }
    DbSet<MatchParticipant> MatchParticipants { get; }
    DbSet<MatchFeedback> MatchFeedbacks { get; }
    DbSet<Report> Reports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}