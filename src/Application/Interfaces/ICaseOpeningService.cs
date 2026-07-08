using CaseSimulator.Application.Common.Models;
using CaseSimulator.Domain.Entities;

namespace CaseSimulator.Application.Interfaces;

public interface ICaseOpeningService
{
    OpenCaseResult OpenCase(Case caseEntity, User user);
}