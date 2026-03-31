namespace PlanIt.Application.Attractions.Results;

public record AttractionResult(
    Guid Id,
    Guid ScheduleId,
    string Name,
    string Description,
    string ImageUrl,
    int Capacity,
    int RemainingCapacity);
