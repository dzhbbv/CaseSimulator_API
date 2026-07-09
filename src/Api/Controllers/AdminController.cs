using CaseSimulator.Application.Admin.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaseSimulator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    private bool IsAdmin()
    {
        Request.Headers.TryGetValue("X-Admin-Key", out var key);
        return key == configuration["AdminSettings:SecretKey"];
    }

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem(
        [FromBody] CreateCaseItemCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsAdmin()) return Forbid();
        var id = await mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPost("cases")]
    public async Task<IActionResult> CreateCase(
        [FromBody] CreateCaseCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsAdmin()) return Forbid();
        var id = await mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPost("cases/{caseId}/items")]
    public async Task<IActionResult> AddItemToCase(
        [FromRoute] Guid caseId,
        [FromBody] AddItemToCaseRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsAdmin()) return Forbid();
        await mediator.Send(
            new AddItemToCaseCommand(caseId, request.CaseItemId, request.DropChance),
            cancellationToken);
        return Ok();
    }
}

public record AddItemToCaseRequest(Guid CaseItemId, decimal DropChance);