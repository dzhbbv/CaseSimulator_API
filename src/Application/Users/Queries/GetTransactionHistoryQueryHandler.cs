using CaseSimulator.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CaseSimulator.Application.Users.Queries;
public class GetTransactionHistoryQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetTransactionHistoryQuery, IReadOnlyCollection<TransactionDto>>
{
    public async Task<IReadOnlyCollection<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var transactions = await dbContext.Transactions
            .Where(t => t.UserId == currentUserService.UserId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TransactionDto(t.Id, t.Amount.Amount, t.CurrentBalance.Amount, t.BalanceAfter.Amount, t.Type, t.CreatedAt))
            .ToListAsync(cancellationToken);
        return transactions.AsReadOnly();
    }
}