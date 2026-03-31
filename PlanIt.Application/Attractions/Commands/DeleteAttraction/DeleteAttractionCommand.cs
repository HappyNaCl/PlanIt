using MediatR;

namespace PlanIt.Application.Attractions.Commands.DeleteAttraction;

public record DeleteAttractionCommand(Guid AttractionId) : IRequest;
