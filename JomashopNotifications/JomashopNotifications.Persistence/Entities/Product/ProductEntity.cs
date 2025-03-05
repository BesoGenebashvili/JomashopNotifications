namespace JomashopNotifications.Persistence.Entities.Product;

public enum ProductStatus : byte
{
    Active = 1,
    Inactive = 2
}

public sealed record ProductEntity
{
    public required int Id { get; init; }
    public required string Brand { get; init; }
    public required string Name { get; set; }
    public required string Link { get; init; }
    public required ProductStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required List<ProductImageEntity> Images { get; init; } = [];
}
