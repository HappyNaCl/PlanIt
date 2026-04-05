namespace PlanIt.Contracts.Me.Response;

public record MyAttractionResponse(
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
