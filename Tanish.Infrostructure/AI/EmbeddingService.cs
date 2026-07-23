using Microsoft.Extensions.AI;
using Pgvector;

namespace Tanish.Infrastructure.AI;

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

    public EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> generator)
    {
        _generator = generator;
    }

    public async Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
    {
        var result = await _generator.GenerateAsync(text, cancellationToken: ct);
        return new Vector(result.Vector);
    }
}