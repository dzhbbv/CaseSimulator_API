using CaseSimulator.Domain.Enums;
namespace CaseSimulator.Application.Users.Queries;
public record TransactionDto(Guid Id, decimal Amount, decimal CurrentBalance, decimal BalanceAfter, TransactionType Type, DateTime CreatedAt);