namespace CaseSimulator.Application.Users.Queries;
public record SaleHistoryDto(Guid Id, Guid CaseItemId, decimal Price, DateTime SoldDate);