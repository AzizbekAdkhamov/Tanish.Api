// Tanish.Application/Profiles/Commands/CreateActivityProfileCommandHandler.cs
using MediatR;
using Tanish.Application.Common.Interfaces;
using Tanish.Domain.Models.Profile;

namespace Tanish.Application.Profiles.Commands;

public class CreateActivityProfileCommandHandler : IRequestHandler<CreateActivityProfileCommand, Guid>
{
    private readonly IAppDbContext _db;
    private readonly IEmbeddingService _embeddingService;

    public CreateActivityProfileCommandHandler(IAppDbContext db, IEmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }

    public async Task<Guid> Handle(CreateActivityProfileCommand request, CancellationToken cancellationToken)
    {
        var normalizedBlurb = request.BlurbText.Trim();

        var embedding = await _embeddingService.GenerateEmbeddingAsync(normalizedBlurb, cancellationToken);

        var profile = new ActivityProfile
        {
            UserId = request.UserId,
            Category = request.Category,
            Level = request.Level,
            Availability = request.Availability,
            BlurbText = normalizedBlurb,
            BlurbEmbedding = embedding,
            IsSearchable = true
        };

        _db.ActivityProfiles.Add(profile);
        await _db.SaveChangesAsync(cancellationToken);

        return profile.Id;
    }
}