using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Application.Users.Queries;

public class GetBalanceQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetBalanceQuery, decimal>
{
    public async Task<decimal> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserService.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User session is invalid or user not found");

        return user.Balance.Amount;
    }
}