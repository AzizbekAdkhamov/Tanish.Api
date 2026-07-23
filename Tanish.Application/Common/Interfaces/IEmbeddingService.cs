using Pgvector;

namespace Tanish.Application.Common.Interfaces;
public interface IEmbeddingService
{
    Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken ct = default);
}
