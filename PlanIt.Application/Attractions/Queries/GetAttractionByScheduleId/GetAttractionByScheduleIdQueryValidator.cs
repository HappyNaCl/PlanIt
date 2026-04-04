using FluentValidation;

namespace PlanIt.Application.Attractions.Queries.GetAttractionByScheduleId;

public class GetAttractionByScheduleIdQueryValidator : AbstractValidator<GetAttractionByScheduleIdQuery>
{
    public GetAttractionByScheduleIdQueryValidator()
    {
        RuleFor(s => s.ScheduleId)
            .NotEmpty().WithMessage("ScheduleId is required");
    }
}