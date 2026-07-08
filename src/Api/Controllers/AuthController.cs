using CaseSimulator.Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaseSimulator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var userId = await mediator.Send(new RegisterUserCommand(
            request.Username,
            request.Email,
            request.Password,
            request.ClientSeed), cancellationToken);

        return Ok(new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(
            request.Email,
            request.Password), cancellationToken);

        return Ok(result);
    }
}

public record RegisterRequest(string Username, string Email, string Password, string ClientSeed);
public record LoginRequest(string Email, string Password);