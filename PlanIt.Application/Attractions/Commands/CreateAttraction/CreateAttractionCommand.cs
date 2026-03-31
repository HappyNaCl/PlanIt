using MediatR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Domain.ValueObjects;

namespace PlanIt.Application.Attractions.Commands.CreateAttraction;

public record CreateAttractionCommand(
    Guid ScheduleId,
    string Name,
    string Description,
    ImageFile ImageFile,
    int Capacity) : IRequest<AttractionResult>;
