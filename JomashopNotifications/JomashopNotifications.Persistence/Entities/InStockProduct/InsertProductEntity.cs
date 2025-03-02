namespace JomashopNotifications.Persistence.Entities.InStockProduct;

public sealed class UpsertInStockProductEntity
{
    public required int ProductId { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
    public required DateTime CheckedAt { get; init; }
}
