using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Authentication.Commands;
using PlanIt.Application.Authentication.Queries;
using PlanIt.Contracts.Authentication;

namespace PlanIt.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(
        ISender mediator,
        IMapper mapper
    ) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = mapper.Map<RegisterCommand>(request);

        var registerResult = await mediator.Send(command);
        
        return Ok(mapper.Map<AuthenticationResponse>(registerResult));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = mapper.Map<LoginQuery>(request);
        
        var loginResult = await mediator.Send(command);
        
        return Ok(mapper.Map<AuthenticationResponse>(loginResult));
    }
}