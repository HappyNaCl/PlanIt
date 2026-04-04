using MediatR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Domain.ValueObjects;

namespace PlanIt.Application.Attractions.Commands.UpdateAttraction;

public record UpdateAttractionCommand(
    Guid AttractionId,
    Guid ScheduleId,
    string Name,
    string Description,
    ImageFile? ImageFile,
    int Capacity) : IRequest<AttractionResult>;
