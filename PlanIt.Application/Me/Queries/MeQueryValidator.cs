using FluentValidation;

namespace PlanIt.Application.Me.Queries;

public class MeQueryValidator : AbstractValidator<MeQuery>
{
    public MeQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}