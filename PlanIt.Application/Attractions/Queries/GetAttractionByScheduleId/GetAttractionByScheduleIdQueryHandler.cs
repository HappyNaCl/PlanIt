using MediatR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Application.Attractions.Queries.GetByScheduleId;

public class GetAttractionByScheduleIdQueryHandler(
    IAttractionRepository attractionRepository,
    IFileUploader fileUploader
    ) : IRequestHandler<GetAttractionByScheduleIdQuery, ICollection<AttractionResult>>
{
    public async Task<ICollection<AttractionResult>> Handle(GetAttractionByScheduleIdQuery request, CancellationToken cancellationToken)
    {
        var attractions = await attractionRepository.GetByScheduleId(request.ScheduleId);

        return attractions.Select(a => new AttractionResult(
            a.Id,
            a.ScheduleId,
            a.Name,
            a.Description,
            $"{fileUploader.GetEndpoint()}/{a.ImageKey}",
            a.Capacity,
            a.Capacity - a.Registrants.Count
        )).ToList();
    }
}