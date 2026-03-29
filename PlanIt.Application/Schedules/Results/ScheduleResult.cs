namespace PlanIt.Application.Schedules.Results;

public record ScheduleResult(
    Guid Id,
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime,
    int AttractionCount);