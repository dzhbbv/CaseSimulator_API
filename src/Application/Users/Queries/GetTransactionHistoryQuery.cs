using MediatR;
namespace CaseSimulator.Application.Users.Queries;
public record GetTransactionHistoryQuery : IRequest<IReadOnlyCollection<TransactionDto>>;