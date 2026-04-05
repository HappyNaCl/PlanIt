namespace PlanIt.Application.Registrants.Results;

public record MyAttractionResult(
    Guid AttractionId,
    string Name,
    string Description,
    string ImageUrl,
    int Capacity,
    int RemainingCapacity,
    Guid ScheduleId,
    string ScheduleName,
    string ScheduleLocation,
    DateTime ScheduleStartTime,
    DateTime ScheduleEndTime);
