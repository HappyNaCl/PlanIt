namespace PlanIt.Application.Attractions.Results;

public record AttractionResult(
    Guid Id,
    Guid ScheduleId,
    string Name,
    string Description,
    string ImageKey,
    int Capacity);
