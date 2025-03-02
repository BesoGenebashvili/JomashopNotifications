using JomashopNotifications.Application.Product.Contracts;

namespace JomashopNotifications.Application.Messages;

public sealed record ProductInStockEvent
{
    public int ProductId { get; init; }
    public int InStockProductId { get; init; }
    public required string Brand { get; init; }
    public required string Name { get; init; }
    public required string Link { get; init; }
    public decimal Price { get; init; }
    public DateTime CheckedAt { get; init; }
    public required IReadOnlyList<ProductImageDto> ProductImages { get; init; } = [];
}
