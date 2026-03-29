using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Me.Queries;
using PlanIt.Contracts.Me.Response;

namespace PlanIt.Api.Controllers;

[ApiController]
[Route("me")]
public class MeController(
    ISender mediator,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim);

        var query = new MeQuery(userId);

        var result = await mediator.Send(query);

        return Ok(mapper.Map<MeResponse>(result));
    }
}