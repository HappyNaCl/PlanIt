using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Attractions.Commands.CreateAttraction;
using PlanIt.Application.Attractions.Commands.DeleteAttraction;
using PlanIt.Application.Attractions.Commands.UpdateAttraction;
using PlanIt.Application.Attractions.Queries.GetAttractionByScheduleId;
using PlanIt.Application.Attractions.Results;
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
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAttractions(Guid scheduleId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetAttractionByScheduleIdQuery(scheduleId, userId);
        
        var results = await mediator.Send(query);
        
        return Ok(results.Select(mapper.Map<DetailedAttractionResult>));
    }
    
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> CreateAttraction(
        Guid scheduleId,
        [FromForm] CreateAttractionRequest request,
        IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("Image file is required.");
        
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

    [HttpPut("{attractionId:guid}")]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> UpdateAttraction(
        Guid scheduleId,
        Guid attractionId,
        [FromForm] UpdateAttractionRequest request,
        IFormFile? imageFile)
    {
        ImageFile? image = null;
        if (imageFile is { Length: > 0 })
        {
            image = new ImageFile(
                imageFile.OpenReadStream(),
                imageFile.FileName,
                imageFile.ContentType);
        }

        var command = new UpdateAttractionCommand(
            attractionId,
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
