using MediatR;

namespace CaseSimulator.Application.Users.Queries;

public record GetBalanceQuery : IRequest<decimal>;