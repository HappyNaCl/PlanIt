using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanIt.Application.Authentication.Commands.RefreshToken;
using PlanIt.Application.Authentication.Commands.Register;
using PlanIt.Application.Authentication.Queries.Login;
using PlanIt.Contracts.Authentication;
using PlanIt.Domain.Common.Exceptions.Authentication;

namespace PlanIt.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(
        ISender mediator,
        IMapper mapper
    ) : ControllerBase
{
    private const string RefreshTokenCookieKey = "refreshToken";
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = mapper.Map<RegisterCommand>(request);

        var registerResult = await mediator.Send(command);
        
        SetRefreshTokenCookie(registerResult.RefreshToken);
        
        return Ok(mapper.Map<AuthenticationResponse>(registerResult));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = mapper.Map<LoginQuery>(request);

        var loginResult = await mediator.Send(command);

        SetRefreshTokenCookie(loginResult.RefreshToken);
        
        return Ok(mapper.Map<AuthenticationResponse>(loginResult));
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.ContainsKey(RefreshTokenCookieKey))
        {
            return Ok();
        }
        
        var refreshToken = Request.Cookies[RefreshTokenCookieKey];
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new MissingRefreshTokenException();
        }
        
        var command = new RefreshTokenCommand(refreshToken);

        var result = await mediator.Send(command);
        
        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(mapper.Map<RefreshTokenResponse>(result));
    }
    
    
    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        if (!Request.Cookies.ContainsKey(RefreshTokenCookieKey))
        {
            return Task.FromResult<IActionResult>(Ok());
        }

        Response.Cookies.Delete(RefreshTokenCookieKey);
        
        return Task.FromResult<IActionResult>(NoContent());
    }


    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,               
            Secure = true,                 
            SameSite = SameSiteMode.Strict, 
            Expires = DateTime.UtcNow.AddDays(30)
        };

        Response.Cookies.Append(RefreshTokenCookieKey, refreshToken, cookieOptions);
    }
}