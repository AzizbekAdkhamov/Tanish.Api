// Tanish.Application/Common/Interfaces/IModerationService.cs
namespace Tanish.Application.Common.Interfaces;

public interface IModerationService
{
    Task<bool> IsAppropriateAsync(string text, CancellationToken ct = default);
}