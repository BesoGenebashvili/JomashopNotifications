using JomashopNotifications.Application.InStockProduct.Contracts;
using JomashopNotifications.Application.Product.Contracts;

namespace JomashopNotifications.Application.Messages;

public sealed record ProductInStockEvent
{
    public required int ProductId { get; init; }
    public required int InStockProductId { get; init; }
    public required string Brand { get; init; }
    public required string Name { get; init; }
    public required string Link { get; init; }
    public required MoneyDto Price { get; init; }
    public required DateTime CheckedAt { get; init; }
    public IReadOnlyList<ProductImageDto> Images { get; init; } = [];
}
