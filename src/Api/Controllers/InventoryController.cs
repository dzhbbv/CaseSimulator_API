using CaseSimulator.Application.Cases.Commands.OpenCase;
using CaseSimulator.Application.Cases.Queries.GetAllCases;
using CaseSimulator.Application.Cases.Queries.GetCaseById;
using CaseSimulator.Application.Users.Commands;
using CaseSimulator.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseSimulator.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id}/sell")]
    public async Task<IActionResult> SaleItem([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var transaction = await mediator.Send(new SaleItemCommand(id), cancellationToken);
        return Ok(transaction);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetInventory(CancellationToken cancellationToken)
    {
        var inventory = await mediator.Send(new GetUserInventoryQuery(), cancellationToken);
        return Ok(inventory);
    }
}