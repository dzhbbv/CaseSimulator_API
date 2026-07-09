using MediatR;
namespace CaseSimulator.Application.Users.Queries;
public record GetSaleHistoryQuery : IRequest<IReadOnlyCollection<SaleHistoryDto>>;