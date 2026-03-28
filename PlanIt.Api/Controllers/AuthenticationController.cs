using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Authentication.Commands.RefreshToken;
using PlanIt.Application.Authentication.Commands.Register;
using PlanIt.Application.Authentication.Queries.Login;
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

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var command = mapper.Map<RefreshTokenCommand>(request);

        var result = await mediator.Send(command);

        return Ok(mapper.Map<RefreshTokenResponse>(result));
    }
}