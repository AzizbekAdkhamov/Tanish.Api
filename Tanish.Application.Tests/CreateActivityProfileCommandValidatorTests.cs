using Tanish.Application.Profiles.Commands;
using Tanish.Domain.Enums;
using Xunit;

namespace Tanish.Application.Tests;

public class CreateActivityProfileCommandValidatorTests
{
    private readonly CreateActivityProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyBlurb_Fails()
    {
        var command = new CreateActivityProfileCommand(
            Guid.NewGuid(), ActivityCategory.Coding, ExperienceLevel.Beginner, "mornings", "", 2);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_GroupSizeAboveCategoryMax_Fails()
    {
        // Coding's max is 4 — 15 should fail
        var command = new CreateActivityProfileCommand(
            Guid.NewGuid(), ActivityCategory.Coding, ExperienceLevel.Beginner, "mornings", "Looking for a coding buddy", 15);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var command = new CreateActivityProfileCommand(
            Guid.NewGuid(), ActivityCategory.Coding, ExperienceLevel.Beginner, "mornings", "Looking for a coding buddy", 2);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}