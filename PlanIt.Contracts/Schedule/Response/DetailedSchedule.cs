namespace PlanIt.Contracts.Schedule.Response;

public record DetailedSchedule(
    Guid Id,
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime,
    ICollection<AttractionDto> Attractions);

public record AttractionDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    int Capacity,
    int RemainingCapacity,
    bool HasJoined);