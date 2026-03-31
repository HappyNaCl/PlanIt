using MediatR;
using PlanIt.Application.Attractions.Results;

namespace PlanIt.Application.Attractions.Queries.GetByScheduleId;

public record GetAttractionByScheduleIdQuery(
    Guid ScheduleId) : IRequest<ICollection<AttractionResult>>;