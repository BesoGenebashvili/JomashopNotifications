namespace JomashopNotifications.Persistence.Entities.Product;

public sealed class InStockProductEntity
{
    public int Id { get; set; }
    public int ProductId { get; init; }
    public decimal Price { get; init; }
    public DateTime CheckedAt { get; init; }
}
