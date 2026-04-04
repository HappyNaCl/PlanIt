using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Schedules.Commands.CreateSchedule;
using PlanIt.Application.Schedules.Commands.DeleteSchedule;
using PlanIt.Application.Schedules.Commands.UpdateSchedule;
using PlanIt.Application.Schedules.Queries.GetScheduleById;
using PlanIt.Application.Schedules.Queries.GetSchedulesByDate;
using PlanIt.Contracts.Schedule.Request;
using PlanIt.Contracts.Schedule.Response;
using PlanIt.Domain.Common.Enums;

namespace PlanIt.Api.Controllers;

[ApiController]
[Route("schedules")]
public class ScheduleController(
    ISender mediator,
    IMapper mapper
    ) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> CreateSchedule(CreateScheduleRequest request)
    {
        var command = mapper.Map<CreateScheduleCommand>(request);

        var result = await mediator.Send(command);

        return Ok(mapper.Map<ScheduleResponse>(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetScheduleById(Guid id)
    {
        var result = await mediator.Send(new GetScheduleByIdQuery(id));
        return Ok(mapper.Map<DetailedSchedule>(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> UpdateSchedule(Guid id, UpdateScheduleRequest request)
    {
        var command = new UpdateScheduleCommand(id, request.Name, request.Description, request.Location);
        var result = await mediator.Send(command);
        return Ok(mapper.Map<ScheduleResponse>(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.ADMIN))]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        await mediator.Send(new DeleteScheduleCommand(id));
        return NoContent();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSchedulesByDate([FromQuery] DateOnly? date, [FromQuery] int utcOffsetMinutes = 0)
    {
        var query = new GetSchedulesByDateQuery(date ?? DateOnly.FromDateTime(DateTime.UtcNow), utcOffsetMinutes);

        var results = await mediator.Send(query);

        return Ok(results.Select(mapper.Map<ScheduleResponse>));
    }
}