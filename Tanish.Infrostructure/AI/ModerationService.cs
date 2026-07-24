using Microsoft.Extensions.AI;
using Tanish.Application.Common.Interfaces;

namespace Tanish.Infrastructure.AI;

public class ModerationService : IModerationService
{
    private readonly IChatClient _chatClient;

    public ModerationService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<bool> IsAppropriateAsync(string text, CancellationToken ct = default)
    {
        var prompt =
            $"""
             You are a content filter for an accountability-partner matching bot.
             This bot is strictly for finding activity partners (coding practice, gym, studying, job hunting, sports, etc.) — it is NOT a dating or relationship app.

             Does the following user description express romantic or dating interest, rather than a platonic activity/accountability partner?
             Answer with exactly one word: "yes" or "no".

             Description: "{text}"
             """;

        var response = await _chatClient.GetResponseAsync(prompt, cancellationToken: ct);
        var answer = response.Text.Trim().ToLowerInvariant();

        // "yes" means it IS romantic/dating content -> NOT appropriate for this bot
        return !answer.StartsWith("yes");
    }
}