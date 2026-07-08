namespace CaseSimulator.Application.Users.Queries;

public record InventoryItemDto(
    Guid Id,
    string ItemName,
    decimal Price,
    string ImageUrl,
    string RarityName,
    string RarityColor
);