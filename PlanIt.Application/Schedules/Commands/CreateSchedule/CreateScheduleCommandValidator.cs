using FluentValidation;

namespace PlanIt.Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.");
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.")
            .Must((cmd, endTime) => endTime.Date == cmd.StartTime.Date)
            .WithMessage("End time must be on the same day as start time.");
    }
}