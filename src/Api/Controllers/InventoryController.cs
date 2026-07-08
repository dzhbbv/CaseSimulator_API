using CaseSimulator.Application.Cases.Commands.OpenCase;
using CaseSimulator.Application.Cases.Queries.GetAllCases;
using CaseSimulator.Application.Cases.Queries.GetCaseById;
using CaseSimulator.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseSimulator.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/inventories")]
public class InventoryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetInventory([FromBody] Guid id, CancellationToken cancellationToken)
    {
        var inventory = await mediator.Send(new GetUserInventoryQuery(), cancellationToken);
        return Ok(inventory);
    }
}