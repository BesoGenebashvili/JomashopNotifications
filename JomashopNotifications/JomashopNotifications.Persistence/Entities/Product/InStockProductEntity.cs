namespace JomashopNotifications.Persistence.Entities.Product;

public sealed class InStockProductEntity
{
    public int Id { get; set; }
    public int ProductId { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
    public DateTime CheckedAt { get; init; }
}
