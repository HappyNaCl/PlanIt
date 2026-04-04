using MediatR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Application.Attractions.Queries.GetAttractionByScheduleId;

public class GetAttractionByScheduleIdQueryHandler(
    IAttractionRepository attractionRepository,
    IRegistrantRepository registrantRepository,
    IFileUploader fileUploader
    ) : IRequestHandler<GetAttractionByScheduleIdQuery, ICollection<DetailedAttractionResult>>
{
    public async Task<ICollection<DetailedAttractionResult>> Handle(GetAttractionByScheduleIdQuery request, CancellationToken cancellationToken)
    {
        var attractions = await attractionRepository.GetByScheduleId(request.ScheduleId);
        var joinedIds = await registrantRepository.GetRegisteredAttractionIds(request.UserId);
        var results = new List<DetailedAttractionResult>();
        foreach (var a in attractions)
        {
            var remaining = await attractionRepository.GetRemainingCapacity(a.Id);
            results.Add(new DetailedAttractionResult(
                a.Id,
                a.ScheduleId,
                a.Name,
                a.Description,
                $"{fileUploader.GetEndpoint()}/{a.ImageKey}",
                a.Capacity,
                remaining,
                joinedIds.Contains(a.Id)
            ));
        }
        return results;
    }
}