using MediatR;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Application.Attractions.Commands.DeleteAttraction;

public class DeleteAttractionCommandHandler(
    IAttractionRepository attractionRepository
    ) : IRequestHandler<DeleteAttractionCommand>
{
    public async Task Handle(DeleteAttractionCommand request, CancellationToken cancellationToken)
    {
        await attractionRepository.Delete(request.AttractionId);
    }
}
