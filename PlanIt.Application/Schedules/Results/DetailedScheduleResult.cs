namespace PlanIt.Application.Schedules.Results;

public record DetailedScheduleResult(
    Guid Id,
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime,
    List<AttractionResultDto> Attractions);

public record AttractionResultDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    int Capacity,
    int RemainingCapacity);