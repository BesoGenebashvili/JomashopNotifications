namespace JomashopNotifications.Persistence.Entities.Product;

public sealed record ProductImageEntity
{
    public bool IsPrimary { get; set; }
    public required byte[] ImageData { get; set; }
}
