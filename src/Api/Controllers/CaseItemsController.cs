using CaseSimulator.Application.CaseItems;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaseSimulator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaseItemsController (IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCaseItems(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetCaseItemsQueryCommand(), cancellationToken));
}