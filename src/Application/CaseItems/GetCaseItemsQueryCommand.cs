using CaseSimulator.Domain.Entities;
using MediatR;

namespace CaseSimulator.Application.CaseItems;

public class GetCaseItemsQueryCommand() : IRequest<IReadOnlyCollection<CaseItem>>;