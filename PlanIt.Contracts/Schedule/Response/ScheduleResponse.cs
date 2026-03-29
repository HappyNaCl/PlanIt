namespace PlanIt.Contracts.Schedule.Response;

public record ScheduleResponse(
    Guid Id,
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime,
    int AttractionCount);