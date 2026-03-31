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
        await scheduleRepository.GetById(request.ScheduleId);

        var imageKey = await fileUploader.UploadAsync(
            request.ImageFile.Content,
            request.ImageFile.FileName,
            request.ImageFile.ContentType,
            "attraction");

        var attraction = new Attraction
        {
            ScheduleId = request.ScheduleId,
            Name = request.Name,
            Description = request.Description,
            ImageKey = imageKey,
            Capacity = request.Capacity,
        };

        var saved = await attractionRepository.Create(attraction);

        return new AttractionResult(
            saved.Id,
            saved.ScheduleId,
            saved.Name,
            saved.Description,
            saved.ImageKey,
            saved.Capacity);
    }
}
