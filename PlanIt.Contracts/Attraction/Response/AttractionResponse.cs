namespace PlanIt.Contracts.Attraction.Response;

public record AttractionResponse(
    Guid Id,
    Guid ScheduleId,
    string Name,
    string Description,
    string ImageKey,
    int Capacity);
