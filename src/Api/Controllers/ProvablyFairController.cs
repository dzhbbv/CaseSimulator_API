using CaseSimulator.Application.Interfaces;
using CaseSimulator.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaseSimulator.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users/provably-fair")]
public sealed class ProvablyFairController(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>Get the commitment that should be recorded before opening the next case.</summary>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == currentUserService.UserId, cancellationToken);
        if (user is null) return NotFound();

        return Ok(new CurrentCommitmentResponse(
            user.CurrentServerSeedHash,
            user.ClientSeed,
            user.CurrentNonce,
            ProvablyFairAlgorithm.Version));
    }

    [HttpPut("client-seed")]
    public async Task<IActionResult> SetClientSeed(
        [FromBody] SetClientSeedRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ClientSeed) || request.ClientSeed.Trim().Length > 128)
            return BadRequest(new { error = "clientSeed must contain 1-128 non-whitespace characters." });

        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == currentUserService.UserId, cancellationToken);
        if (user is null) return NotFound();

        user.SetClientSeed(request.ClientSeed);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new CurrentCommitmentResponse(
            user.CurrentServerSeedHash, user.ClientSeed, user.CurrentNonce, ProvablyFairAlgorithm.Version));
    }

    /// <summary>
    /// Rotate the seed. The previous seed is revealed only after its commitment has been retired.
    /// Save the current commitment before opening cases if you want an external audit trail.
    /// </summary>
    [HttpPost("rotate")]
    public async Task<IActionResult> Rotate(CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == currentUserService.UserId, cancellationToken);
        if (user is null) return NotFound();

        var revealedSeed = user.CurrentServerSeed;
        var revealedHash = user.CurrentServerSeedHash;
        var newSeed = ProvablyFairAlgorithm.GenerateServerSeed();
        var newHash = ProvablyFairAlgorithm.ComputeServerSeedHash(newSeed);

        user.RotateServerSeed(newSeed, newHash);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new RotateServerSeedResponse(
            revealedSeed,
            revealedHash,
            newHash,
            user.ClientSeed,
            user.CurrentNonce,
            ProvablyFairAlgorithm.Version));
    }

    /// <summary>Returns the authenticated user's last 100 rounds. Active seed values remain hidden.</summary>
    [HttpGet("rounds")]
    public async Task<IActionResult> GetRounds(CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == currentUserService.UserId, cancellationToken);
        if (user is null) return NotFound();

        var rounds = await dbContext.ProvablyFairRounds.AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        return Ok(rounds.Select(round => new ProvablyFairRoundResponse(
            round.Id,
            round.CaseId,
            round.CaseItemId,
            round.ServerSeedHash == user.CurrentServerSeedHash ? null : round.ServerSeed,
            round.ServerSeedHash,
            round.ClientSeed,
            round.Nonce,
            round.DropTableJson,
            round.Algorithm,
            round.CreatedAt,
            round.ServerSeedHash == user.CurrentServerSeedHash || round.Algorithm != ProvablyFairAlgorithm.Version
                ? null
                : ProvablyFairAlgorithm.VerifyRound(
                    round.ServerSeed, round.ServerSeedHash, round.ClientSeed, round.Nonce,
                    round.DropTableJson, round.CaseItemId))));
    }
}

public sealed record SetClientSeedRequest(string ClientSeed);
public sealed record CurrentCommitmentResponse(
    string ServerSeedHash,
    string ClientSeed,
    int NextNonce,
    string Algorithm);
public sealed record RotateServerSeedResponse(
    string RevealedServerSeed,
    string RevealedServerSeedHash,
    string NewServerSeedHash,
    string ClientSeed,
    int NextNonce,
    string Algorithm);
public sealed record ProvablyFairRoundResponse(
    Guid RoundId,
    Guid CaseId,
    Guid CaseItemId,
    string? RevealedServerSeed,
    string ServerSeedHash,
    string ClientSeed,
    int Nonce,
    string DropTableJson,
    string Algorithm,
    DateTime CreatedAt,
    bool? IsVerified);
