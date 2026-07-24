using Tanish.Domain.Enums;
using Tanish.Domain.Rules;
using Xunit;

namespace Tanish.Application.Tests;

public class ActivityGroupSizeRulesTests
{
    [Theory]
    [InlineData(ActivityCategory.Coding, 2, 12)]
    [InlineData(ActivityCategory.OutdoorActivities, 2, 25)]
    [InlineData(ActivityCategory.Studying, 2, 8)]
    public void GetRange_ReturnsExpectedBoundsForCategory(ActivityCategory category, int expectedMin, int expectedMax)
    {
        var (min, max) = ActivityGroupSizeRules.GetRange(category);

        Assert.Equal(expectedMin, min);
        Assert.Equal(expectedMax, max);
    }
}