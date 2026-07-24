using MediatR;
using Serilog.Context;
using Tanish.Application.Matching.Commands;
using Tanish.Application.Matching.Queries;
using Tanish.Application.Profiles;
using Tanish.Application.Profiles.Commands;
using Tanish.Application.Profiles.Queries;
using Tanish.Application.Users.Commands;
using Tanish.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
        if (update.CallbackQuery is { } callbackQuery)
        {
            await HandleCallbackAsync(callbackQuery, ct);
            return;
        }

        if (update.Message?.Text is not { } messageText || update.Message.From is null)
            return;

        var telegramUser = update.Message.From;
        using var _ = LogContext.PushProperty("TelegramUserId", telegramUser.Id);
        var telegramId = telegramUser.Id;
        var chatId = update.Message.Chat.Id;
        var state = _stateStore.Get(telegramId);
        var alias = telegramUser.Username ?? telegramUser.FirstName;

        if (messageText == "/start")
        {
            _stateStore.Reset(telegramId);
            await _mediator.Send(new GetOrCreateUserCommand(telegramId, alias), ct);
            await Send(chatId, "Welcome to Tanish. This bot helps you find an accountability partner - not a dating app.\n\nSend /newprofile to create a profile, /find to search, /stop to leave the searchable pool, or /report to flag your last match.", ct);
            return;
        }

        if (messageText == "/newprofile")
        {
            _stateStore.Reset(telegramId);
            state.Step = ConversationStep.AwaitingCategory;
            await SendCategoryOptions(chatId, ct);
            return;
        }

        if (messageText == "/find")
        {
            await HandleFindAsync(chatId, telegramId, ct);
            return;
        }

        if (messageText == "/stop")
        {
            var userId = await _mediator.Send(new GetOrCreateUserCommand(telegramId, alias), ct);
            var count = await _mediator.Send(new StopSearchingCommand(userId), ct);
            await Send(chatId, count > 0
                ? $"You've been removed from the searchable pool ({count} profile(s))."
                : "You weren't in the searchable pool.", ct);
            return;
        }

        if (messageText == "/report")
        {
            var userId = await _mediator.Send(new GetOrCreateUserCommand(telegramId, alias), ct);
            var recentMatch = await _mediator.Send(new GetMostRecentMatchForUserQuery(userId), ct);

            if (recentMatch is null)
            {
                await Send(chatId, "No recent match found to report.", ct);
                return;
            }

            state.PendingReportMatchId = recentMatch.MatchId;
            state.PendingReporterProfileId = recentMatch.ReporterProfileId;
            state.PendingReportedProfileId = recentMatch.ReportedProfileId;
            state.Step = ConversationStep.AwaitingReportReason;
            await Send(chatId, $"Reporting your match with {recentMatch.ReportedAlias}. Please describe what happened:", ct);
            return;
        }

        switch (state.Step)
        {
            case ConversationStep.AwaitingAvailability:
                state.Availability = messageText.Trim();
                state.Step = ConversationStep.AwaitingBlurb;
                await Send(chatId, "Describe what you're looking for in a partner (a couple sentences is great).", ct);
                return;

            case ConversationStep.AwaitingBlurb:
                state.BlurbText = messageText.Trim();
                state.Step = ConversationStep.None;
                await CreateProfileAsync(chatId, telegramId, alias, state, ct);
                _stateStore.Reset(telegramId);
                return;

            case ConversationStep.AwaitingReportReason:
                await _mediator.Send(new CreateReportCommand(
                    state.PendingReportMatchId!.Value,
                    state.PendingReporterProfileId!.Value,
                    state.PendingReportedProfileId!.Value,
                    messageText.Trim()), ct);
                await Send(chatId, "Thanks - your report has been recorded.", ct);
                _stateStore.Reset(telegramId);
                return;

            default:
                await Send(chatId, "Send /newprofile to create a profile, or /find to search for a partner.", ct);
                return;
        }
    }

    private async Task HandleCallbackAsync(CallbackQuery callback, CancellationToken ct)
    {
        if (callback.Message is null || callback.From is null || callback.Data is null)
            return;

        var telegramId = callback.From.Id;
        var chatId = callback.Message.Chat.Id;
        var state = _stateStore.Get(telegramId);

        await _bot.AnswerCallbackQuery(callback.Id, cancellationToken: ct);

        var (prefix, value) = Split(callback.Data);

        switch (prefix)
        {
            case "cat":
                state.Category = Enum.Parse<ActivityCategory>(value);
                state.Step = ConversationStep.AwaitingLevel;
                await SendLevelOptions(chatId, ct);
                return;

            case "lvl":
                state.Level = Enum.Parse<ExperienceLevel>(value);
                state.Step = ConversationStep.AwaitingAvailability;
                await Send(chatId, "When are you usually available? (e.g. 'mornings', 'weekday evenings')", ct);
                return;

            case "findprofile":
                var profileId = Guid.Parse(value);
                await RunMatchSearch(chatId, telegramId, profileId, ct);
                return;
        }
    }

    private async Task SendCategoryOptions(long chatId, CancellationToken ct)
    {
        var buttons = Enum.GetValues<ActivityCategory>()
            .Select(c => InlineKeyboardButton.WithCallbackData(c.ToString(), $"cat:{c}"))
            .Chunk(2)
            .Select(row => row.ToArray())
            .ToArray();

        await _bot.SendMessage(chatId, "What are you looking for a partner for?",
            replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: ct);
    }

    private async Task SendLevelOptions(long chatId, CancellationToken ct)
    {
        var buttons = Enum.GetValues<ExperienceLevel>()
            .Select(l => InlineKeyboardButton.WithCallbackData(l.ToString(), $"lvl:{l}"))
            .Chunk(2)
            .Select(row => row.ToArray())
            .ToArray();

        await _bot.SendMessage(chatId, "What's your level?",
            replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: ct);
    }

    private async Task CreateProfileAsync(long chatId, long telegramId, string alias, ConversationState state, CancellationToken ct)
    {
        try
        {
            var userId = await _mediator.Send(new GetOrCreateUserCommand(telegramId, alias), ct);

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

        var profiles = await _mediator.Send(new GetSearchableProfilesForUserQuery(userId), ct);

        if (profiles.Count == 0)
        {
            await Send(chatId, "You don't have an active profile yet. Send /newprofile first.", ct);
            return;
        }

        if (profiles.Count == 1)
        {
            await RunMatchSearch(chatId, telegramId, profiles[0].ProfileId, ct);
            return;
        }

        var buttons = profiles
            .Select(p => InlineKeyboardButton.WithCallbackData(p.Category.ToString(), $"findprofile:{p.ProfileId}"))
            .Chunk(2)
            .Select(row => row.ToArray())
            .ToArray();

        await _bot.SendMessage(chatId, "Which profile do you want to search with?",
            replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: ct);
    }

    private async Task RunMatchSearch(long chatId, long telegramId, Guid profileId, CancellationToken ct)
    {
        var candidates = await _mediator.Send(new FindMatchCandidatesQuery(profileId, TopN: 1), ct);

        if (candidates.Count == 0)
        {
            await Send(chatId, "No matches found right now. Try again later - we'll keep looking.", ct);
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
            var candidateProfileId = state.PendingCandidateIds[0];
            var participantIds = new List<Guid> { state.ActiveProfileId!.Value, candidateProfileId };

            await _mediator.Send(new CreateMatchCommand(participantIds), ct);
            await Send(chatId, "You're matched! Reach out and get started.", ct);

            var otherTelegramId = await _mediator.Send(new GetProfileOwnerTelegramIdQuery(candidateProfileId), ct);
            if (otherTelegramId is not null)
            {
                await Send(otherTelegramId.Value, "You've been matched with a new accountability partner! Send /find again anytime to search for more.", ct);
            }
        }
        else
        {
            await Send(chatId, "No problem - send /find anytime to search again.", ct);
        }

        _stateStore.Reset(telegramId);
    }

    private static (string prefix, string value) Split(string data)
    {
        var parts = data.Split(':', 2);
        return (parts[0], parts.Length > 1 ? parts[1] : "");
    }

    private Task Send(long chatId, string text, CancellationToken ct) =>
        _bot.SendMessage(chatId, text, cancellationToken: ct);
}