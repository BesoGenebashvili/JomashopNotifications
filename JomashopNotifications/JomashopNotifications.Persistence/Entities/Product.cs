namespace JomashopNotifications.Persistence.Entities;

public enum ProductStatus : byte
{
    Active = 1,
    Inactive = 2
}

public sealed record InsertProductModel
{
    public required Uri Link { get; init; }
    public required ProductStatus Status { get; init; }
}

public sealed record Product
{
    public int Id { get; init; }
    public required string Link { get; init; }
    public ProductStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
