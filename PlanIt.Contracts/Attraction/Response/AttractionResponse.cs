namespace PlanIt.Contracts.Attraction.Response;

public record AttractionResponse(
    Guid Id,
    Guid ScheduleId,
    string Name,
    string Description,
    string ImageUrl,
    int Capacity,
    int RemainingCapacity);
