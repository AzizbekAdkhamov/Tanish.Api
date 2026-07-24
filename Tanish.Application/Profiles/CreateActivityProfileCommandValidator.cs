using FluentValidation;
using Tanish.Domain.Rules;

namespace Tanish.Application.Profiles;

public class CreateActivityProfileCommandValidator : AbstractValidator<CreateActivityProfileCommand>
{
    public CreateActivityProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.BlurbText)
            .NotEmpty().WithMessage("Please describe what you're looking for.")
            .MaximumLength(1000);

        RuleFor(x => x.Availability)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.Level)
            .IsInEnum();

        RuleFor(x => x)
            .Must(x =>
            {
                var (min, max) = ActivityGroupSizeRules.GetRange(x.Category);
                return x.DesiredGroupSize >= min && x.DesiredGroupSize <= max;
            })
            .WithMessage(x =>
            {
                var (min, max) = ActivityGroupSizeRules.GetRange(x.Category);
                return $"For {x.Category}, group size must be between {min} and {max}.";
            });
    }
}