using CaseSimulator.Application.Cases.Commands.OpenCase;
using CaseSimulator.Application.Cases.Queries.GetAllCases;
using CaseSimulator.Application.Cases.Queries.GetCaseById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseSimulator.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CasesController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost("{id}/open")]
    public async Task<IActionResult> OpenCase([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var caseItemId = await mediator.Send(new OpenCaseCommand(id), cancellationToken);
        return Ok(caseItemId);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCaseById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var caseEntityDto = await mediator.Send(new GetCaseByIdQuery(id), cancellationToken);
        return Ok(caseEntityDto);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCases(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAllCasesQuery(), cancellationToken));
}