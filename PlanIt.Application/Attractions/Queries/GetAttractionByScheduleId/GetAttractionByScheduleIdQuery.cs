using MediatR;
using PlanIt.Application.Attractions.Results;

namespace PlanIt.Application.Attractions.Queries.GetAttractionByScheduleId;

public record GetAttractionByScheduleIdQuery(
    Guid ScheduleId,
    Guid UserId) : IRequest<ICollection<DetailedAttractionResult>>;