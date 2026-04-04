using MediatR;
using PlanIt.Application.Attractions.Results;
using PlanIt.Application.Common.Interfaces.FileUploader;
using PlanIt.Application.Common.Interfaces.Persistence;

namespace PlanIt.Application.Attractions.Commands.UpdateAttraction;

public class UpdateAttractionCommandHandler(
    IAttractionRepository attractionRepository,
    IFileUploader fileUploader
    ) : IRequestHandler<UpdateAttractionCommand, AttractionResult>
{
    public async Task<AttractionResult> Handle(UpdateAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = await attractionRepository.GetByIdForUpdate(request.AttractionId);

        string imageKey = attraction.ImageKey;
        if (request.ImageFile is not null)
        {
            await fileUploader.DeleteAsync(attraction.ImageKey);
            imageKey = await fileUploader.UploadAsync(
                request.ImageFile.Content,
                request.ImageFile.FileName,
                request.ImageFile.ContentType,
                "attraction");
        }

        attraction.Name = request.Name;
        attraction.Description = request.Description;
        attraction.ImageKey = imageKey;
        attraction.Capacity = request.Capacity;

        var saved = await attractionRepository.Update(attraction);
        var remaining = await attractionRepository.GetRemainingCapacity(saved.Id);

        return new AttractionResult(
            saved.Id,
            saved.ScheduleId,
            saved.Name,
            saved.Description,
            $"{fileUploader.GetEndpoint()}{saved.ImageKey}",
            saved.Capacity,
            remaining);
    }
}