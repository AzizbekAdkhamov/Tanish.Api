using Tanish.Domain.Enums;

namespace Tanish.Domain.Rules;

public static class ActivityGroupSizeRules
{
    public static (int Min, int Max) GetRange(ActivityCategory category) => category switch
    {
        ActivityCategory.Coding => (2, 12),
        ActivityCategory.Studying => (2, 8),
        ActivityCategory.JobHunt => (2, 20),
        ActivityCategory.LanguageLearner => (2, 7),
        ActivityCategory.BookLover => (2, 8),
        ActivityCategory.Fitness => (2, 8),
        ActivityCategory.OutdoorActivities => (2, 25),
        ActivityCategory.ApartmentShare => (2, 8),
        ActivityCategory.Gaming => (2, 10),
        ActivityCategory.OtherInterest => (2, 10),
        _ => (2, 10)
    };
}