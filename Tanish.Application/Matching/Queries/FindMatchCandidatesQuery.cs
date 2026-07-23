// Tanish.Application/Matching/Queries/FindMatchCandidatesQuery.cs
using MediatR;
using Tanish.Domain.Enums;

namespace Tanish.Application.Matching.Queries;

public record FindMatchCandidatesQuery(Guid ProfileId, int TopN = 5) : IRequest<List<MatchCandidateDto>>;

public record MatchCandidateDto(
    Guid ProfileId,
    string Alias,
    ActivityCategory Category,
    ExperienceLevel Level,
    string Availability,
    double SimilarityDistance
);