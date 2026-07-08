using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Common.Models;

public record OpenCaseResult(CaseItem CaseItem, ProvablyFairRound Round);