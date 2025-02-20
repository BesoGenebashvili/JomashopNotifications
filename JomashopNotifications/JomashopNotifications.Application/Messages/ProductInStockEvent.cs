namespace JomashopNotifications.Application.Messages;

public sealed record ProductInStockEvent
{
    public int InStockProductId { get; init; }
    public int ProductId { get; init; }
    public required string Link { get; init; }
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CheckedAt { get; init; }
}
