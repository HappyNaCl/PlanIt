using FluentValidation;

namespace PlanIt.Application.Registrants.Commands;

public class JoinAttractionCommandValidator : AbstractValidator<JoinAttractionCommand>
{
    public JoinAttractionCommandValidator()
    {
        RuleFor(command => command.AttractionId)
            .NotEmpty().WithMessage("The AttractionId is required.");
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("The UserId is required.");
    }
}