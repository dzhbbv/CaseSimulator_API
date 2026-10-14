using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Entities;
using CaseSimulator.Domain.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CaseSimulator.Domain.Services;

namespace CaseSimulator.Application.Users.Commands;

public class RegisterUserCommandHandler(IApplicationDbContext dbContext, IPasswordService hasher) : IRequestHandler<RegisterUserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (exists)
            throw new AlreadyExistingException("User already exists");

        var serverSeed = ProvablyFairAlgorithm.GenerateServerSeed();
        var serverSeedHash = ProvablyFairAlgorithm.ComputeServerSeedHash(serverSeed);

        var user = new User(
            request.Username,
            hasher.Hash(request.Password),
            request.Email,
            request.ClientSeed,
            serverSeed,
            serverSeedHash);

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}