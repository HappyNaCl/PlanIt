using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Registrants.Commands;

namespace PlanIt.Api.Controllers;

[ApiController]
[Route("schedules/{scheduleId:guid}/attractions/{attractionId:guid}/registrants")]
public class RegistrantController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> JoinAttraction(Guid scheduleId, Guid attractionId)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault()
                             ?? Guid.NewGuid().ToString();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await mediator.Send(new JoinAttractionCommand(attractionId, scheduleId, userId, idempotencyKey));

        return Accepted();
    }
}
