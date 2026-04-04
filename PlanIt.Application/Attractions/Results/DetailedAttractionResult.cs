namespace PlanIt.Application.Attractions.Results;

public record DetailedAttractionResult(
    Guid Id,
    Guid ScheduleId,
    string Name,
    string Description,
    string ImageUrl,
    int Capacity,
    int RemainingCapacity,
    bool HasJoined);