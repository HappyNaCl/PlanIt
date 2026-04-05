using MediatR;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Registrants.Results;

namespace PlanIt.Application.Registrants.Queries.GetMyAttractions;

public class GetMyAttractionsQueryHandler(
    IRegistrantRepository registrantRepository,
    IFileUploader fileUploader
) : IRequestHandler<GetMyAttractionsQuery, ICollection<MyAttractionResult>>
{
    public async Task<ICollection<MyAttractionResult>> Handle(GetMyAttractionsQuery request, CancellationToken cancellationToken)
    {
        var attractions = await registrantRepository.GetUserAttractionsWithDetails(request.UserId);
        var endpoint = fileUploader.GetEndpoint();

        return attractions
            .Select(a => a with { ImageUrl = $"{endpoint}{a.ImageUrl}" })
            .ToList();
    }
}
