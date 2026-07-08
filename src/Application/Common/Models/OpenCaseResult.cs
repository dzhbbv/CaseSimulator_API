using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Common.Models;

public class OpenCaseResult
{
    public CaseItem CaseItem { get; protected set; }
    public ProvablyFairRound Round { get; protected set; }

    public OpenCaseResult(CaseItem caseItem, ProvablyFairRound round)
    {
        CaseItem = caseItem;
        Round = round;
    }
}