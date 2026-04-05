using MediatR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Domain.Entities;

namespace PlanIt.Application.Attractions.Commands.CreateAttraction;

public class CreateAttractionCommandHandler(
    IAttractionRepository attractionRepository,
    IScheduleRepository scheduleRepository,
    IFileUploader fileUploader
    ) : IRequestHandler<CreateAttractionCommand, AttractionResult>
{
    public async Task<AttractionResult> Handle(CreateAttractionCommand request, CancellationToken cancellationToken)
    {
        var imageKey = await fileUploader.UploadAsync(
            request.ImageFile.Content,
            request.ImageFile.FileName,
            request.ImageFile.ContentType,
            "attraction");

        var attraction = Attraction.Create(request.Name, request.Description, imageKey, request.Capacity, request.ScheduleId);

        var saved = await attractionRepository.Create(attraction);

        return new AttractionResult(
            saved.Id,
            saved.ScheduleId,
            saved.Name,
            saved.Description,
            $"{fileUploader.GetEndpoint()}{saved.ImageKey}",
            saved.Capacity,
            saved.Capacity);
    }
}
