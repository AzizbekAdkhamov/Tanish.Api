using Tanish.Domain.Enums;

namespace Tanish.Api.TelegramBot;

public static class CategoryStyle
{
    public static string Emoji(ActivityCategory category) => category switch
    {
        ActivityCategory.Coding => "💻",
        ActivityCategory.Studying => "📚",
        ActivityCategory.JobHunt => "🎯",
        ActivityCategory.LanguageLearner => "🗣️",
        ActivityCategory.BookLover => "📖",
        ActivityCategory.Fitness => "💪",
        ActivityCategory.OutdoorActivities => "⚽",
        ActivityCategory.ApartmentShare => "🏠",
        ActivityCategory.Gaming => "🎮",
        ActivityCategory.OtherInterest => "✨",
        _ => "✨"
    };

    public static string Label(ActivityCategory category) => $"{Emoji(category)} {category}";
}