namespace PlanIt.Contracts.Schedule.Request;

public record CreateScheduleRequest(
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime);
    