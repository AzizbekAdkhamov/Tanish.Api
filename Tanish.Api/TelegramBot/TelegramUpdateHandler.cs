using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Tanish.Application.Matching.Commands;
using Tanish.Application.Matching.Queries;
using Tanish.Application.Profiles.Commands;
using Tanish.Application.Users.Commands;
using Tanish.Domain.Enums;
using Tanish.Application.Profiles.Queries;
using Serilog.Context;

namespace Tanish.Api.TelegramBot;

public class TelegramUpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly IMediator _mediator;
    private readonly IConversationStateStore _stateStore;

    public TelegramUpdateHandler(ITelegramBotClient bot, IMediator mediator, IConversationStateStore stateStore)
    {
        _bot = bot;
        _mediator = mediator;
        _stateStore = stateStore;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken ct)
    {
        if (update.Message?.Text is not { } messageText || update.Message.From is null)
            return;

        using var _ = LogContext.PushProperty("TelegramUserId", update.Message.From.Id);
        var telegramId = update.Message.From.Id;
        var chatId = update.Message.Chat.Id;
        var state = _stateStore.Get(telegramId);

        if (messageText == "/start")
        {
            _stateStore.Reset(telegramId);
            await Send(chatId, "Welcome to Tanish. This bot helps you find an accountability partner — not a dating app.\n\nSend /newprofile to create a profile, or /find to search for a partner.", ct);
            return;
        }

        if (messageText == "/newprofile")
        {
            _stateStore.Reset(telegramId);
            state.Step = ConversationStep.AwaitingCategory;
            var options = string.Join(", ", Enum.GetNames<ActivityCategory>());
            await Send(chatId, $"What are you looking for a partner for? Options:\n{options}", ct);
            return;
        }

        if (messageText == "/find")
        {
            await HandleFindAsync(chatId, telegramId, ct);
            return;
        }

        switch (state.Step)
        {
            case ConversationStep.AwaitingCategory:
                if (!Enum.TryParse<ActivityCategory>(messageText, true, out var category))
                {
                    await Send(chatId, "Didn't recognize that category, please try again.", ct);
                    return;
                }
                state.Category = category;
                state.Step = ConversationStep.AwaitingLevel;
                await Send(chatId, $"What's your level? Options: {string.Join(", ", Enum.GetNames<ExperienceLevel>())}", ct);
                return;

            case ConversationStep.AwaitingLevel:
                if (!Enum.TryParse<ExperienceLevel>(messageText, true, out var level))
                {
                    await Send(chatId, "Didn't recognize that level, please try again.", ct);
                    return;
                }
                state.Level = level;
                state.Step = ConversationStep.AwaitingAvailability;
                await Send(chatId, "When are you usually available? (e.g. 'mornings', 'weekday evenings')", ct);
                return;

            case ConversationStep.AwaitingAvailability:
                state.Availability = messageText.Trim();
                state.Step = ConversationStep.AwaitingBlurb;
                await Send(chatId, "Describe what you're looking for in a partner (a couple sentences is great).", ct);
                return;

            case ConversationStep.AwaitingBlurb:
                state.BlurbText = messageText.Trim();
                state.Step = ConversationStep.None;
                await CreateProfileAsync(chatId, telegramId, state, ct);
                _stateStore.Reset(telegramId);
                return;

            case ConversationStep.AwaitingMatchConfirmation:
                await HandleMatchConfirmationAsync(chatId, telegramId, state, messageText, ct);
                return;

            default:
                await Send(chatId, "Send /newprofile to create a profile, or /find to search for a partner.", ct);
                return;
        }
    }

    private async Task CreateProfileAsync(long chatId, long telegramId, ConversationState state, CancellationToken ct)
    {
        try
        {
            var userId = await _mediator.Send(new GetOrCreateUserCommand(telegramId, null), ct);

            var command = new CreateActivityProfileCommand(
                UserId: userId,
                Category: state.Category!.Value,
                Level: state.Level!.Value,
                Availability: state.Availability!,
                BlurbText: state.BlurbText!
            );

            await _mediator.Send(command, ct);
            await Send(chatId, "Profile created! Send /find when you're ready to search for a partner.", ct);
        }
        catch (Exception ex)
        {
            await Send(chatId, $"Something went wrong: {ex.Message}", ct);
        }
    }

    private async Task HandleFindAsync(long chatId, long telegramId, CancellationToken ct)
    {
        var userId = await _mediator.Send(new GetOrCreateUserCommand(telegramId, null), ct);

        // NOTE: placeholder lookup — see explanation below the code
        var profileId = await _mediator.Send(new GetLatestProfileIdQuery(userId), ct);
        if (profileId is null)
        {
            await Send(chatId, "You don't have a profile yet. Send /newprofile first.", ct);
            return;
        }

        var candidates = await _mediator.Send(new FindMatchCandidatesQuery(profileId.Value, TopN: 1), ct);

        if (candidates.Count == 0)
        {
            await Send(chatId, "No matches found right now. Try again later — we'll keep looking.", ct);
            return;
        }

        var best = candidates[0];
        var state = _stateStore.Get(telegramId);
        state.ActiveProfileId = profileId;
        state.PendingCandidateIds = new List<Guid> { best.ProfileId };
        state.Step = ConversationStep.AwaitingMatchConfirmation;

        await Send(chatId,
            $"Found a potential match: {best.Alias}, {best.Level}, available {best.Availability}.\n\nReply 'yes' to connect, or 'no' to skip.", ct);
    }

    private async Task HandleMatchConfirmationAsync(long chatId, long telegramId, ConversationState state, string messageText, CancellationToken ct)
    {
        if (messageText.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            var participantIds = new List<Guid> { state.ActiveProfileId!.Value };
            participantIds.AddRange(state.PendingCandidateIds);

            var matchId = await _mediator.Send(new CreateMatchCommand(participantIds), ct);
            await Send(chatId, "You're matched! Reach out and get started.", ct);
        }
        else
        {
            await Send(chatId, "No problem — send /find anytime to search again.", ct);
        }

        _stateStore.Reset(telegramId);
    }

    private Task Send(long chatId, string text, CancellationToken ct) =>
        _bot.SendMessage(chatId, text, cancellationToken: ct);
}