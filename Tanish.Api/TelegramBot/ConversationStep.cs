using Tanish.Domain.Enums;

namespace Tanish.Api.TelegramBot;

public enum ConversationStep
{
    None,
    AwaitingCategory,
    AwaitingLevel,
    AwaitingAvailability,
    AwaitingBlurb,
    AwaitingPhoto,
    AwaitingFindProfileSelection,
    AwaitingMatchConfirmation,
    AwaitingReportReason
}