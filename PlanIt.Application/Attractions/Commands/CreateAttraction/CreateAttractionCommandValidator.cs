using FluentValidation;

namespace PlanIt.Application.Attractions.Commands.CreateAttraction;

public class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("ScheduleId is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(150).WithMessage("Description must not exceed 150 characters.");
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
    }
}
