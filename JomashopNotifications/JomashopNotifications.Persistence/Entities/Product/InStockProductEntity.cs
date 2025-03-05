namespace JomashopNotifications.Persistence.Entities.Product;

public sealed class InStockProductEntity
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; } = default!;
    public required DateTime CheckedAt { get; init; }
}
