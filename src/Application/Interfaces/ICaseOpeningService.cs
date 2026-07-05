using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Interfaces;

public interface ICaseOpeningService
{
    CaseItem OpenCase(Case caseEntity);
}