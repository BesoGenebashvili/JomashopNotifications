namespace JomashopNotifications.Persistence.Entities.Product;

public sealed record InsertProductEntity
{
    public required string Brand { get; init; }
    public required string Name { get; init; }
    public required string Link { get; init; }
    public required ProductStatus Status { get; init; }
    public required List<ProductImageEntity> Images { get; init; } = [];
}
