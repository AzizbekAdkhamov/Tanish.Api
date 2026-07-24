using FluentValidation;

namespace Tanish.Application.Profiles.Commands;

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

        RuleFor(x => x.DesiredGroupSize)
            .InclusiveBetween(2, 20).WithMessage("Group size must be between 2 and 20.");
    }
}