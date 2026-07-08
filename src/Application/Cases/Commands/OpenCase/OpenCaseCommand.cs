using MediatR;
namespace CaseSimulator.Application.Cases.Commands.OpenCase;

public class OpenCaseCommand : IRequest<Guid>
{
    public Guid CaseId { get; init; }
    
    public OpenCaseCommand(Guid caseId)
    {
        CaseId = caseId;
    }
}
