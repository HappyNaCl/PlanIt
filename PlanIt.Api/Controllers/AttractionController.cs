using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Attractions.Commands.CreateAttraction;
using PlanIt.Application.Attractions.Commands.DeleteAttraction;
using PlanIt.Contracts.Attraction.Request;
using PlanIt.Contracts.Attraction.Response;
using PlanIt.Domain.Common.Enums;
using PlanIt.Domain.ValueObjects;

namespace PlanIt.Api.Controllers;

[ApiController]
[Route("schedules/{scheduleId:guid}/attractions")]
public class AttractionController(
    ISender mediator,
    IMapper mapper
    ) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> CreateAttraction(
        Guid scheduleId,
        [FromForm] CreateAttractionRequest request,
        IFormFile imageFile)
    {
        var image = new ImageFile(
            imageFile.OpenReadStream(),
            imageFile.FileName,
            imageFile.ContentType);

        var command = new CreateAttractionCommand(
            scheduleId,
            request.Name,
            request.Description,
            image,
            request.Capacity);

        var result = await mediator.Send(command);

        return Ok(mapper.Map<AttractionResponse>(result));
    }

    [HttpDelete("{attractionId:guid}")]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> DeleteAttraction(Guid attractionId)
    {
        await mediator.Send(new DeleteAttractionCommand(attractionId));
        return NoContent();
    }
}
