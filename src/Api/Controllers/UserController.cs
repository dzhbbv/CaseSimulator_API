using CaseSimulator.Application.Users.Commands;
using CaseSimulator.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetBalanceQuery(), cancellationToken));

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventory(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetUserInventoryQuery(), cancellationToken));

    [HttpPost("inventory/{id}/sell")]
    public async Task<IActionResult> SellItem([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var transaction = await mediator.Send(new SaleItemCommand(id), cancellationToken);
        return Ok(transaction);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetTransactionHistoryQuery(), cancellationToken));

    [HttpGet("sales")]
    public async Task<IActionResult> GetSales(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetSaleHistoryQuery(), cancellationToken));
}