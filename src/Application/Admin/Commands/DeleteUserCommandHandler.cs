using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CaseSimulator.Application.Admin.Commands;
public class DeleteUserCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
                       .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
                   ?? throw new NotFoundException("User not found");
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}