using CaseSimulator.Domain.Entities;
using MediatR;

namespace CaseSimulator.Application.Users.Commands;

public record SaleItemCommand(Guid ItemId) : IRequest<Transaction>;