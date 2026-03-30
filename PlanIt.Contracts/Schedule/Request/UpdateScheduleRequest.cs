namespace PlanIt.Contracts.Schedule.Request;

public record UpdateScheduleRequest(
    string Name,
    string Description,
    string Location);
