namespace JomashopNotifications.Persistence.Entities.Product;

public sealed record ProductImageEntity
{
    public required bool IsPrimary { get; init; }
    public required byte[] ImageData { get; init; }
}
