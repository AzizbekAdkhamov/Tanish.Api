using Pgvector;

namespace Tanish.Infrastructure.AI;
public interface IEmbeddingService
{
    Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken ct = default);
}
