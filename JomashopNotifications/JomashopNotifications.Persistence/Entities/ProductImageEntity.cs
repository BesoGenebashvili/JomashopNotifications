namespace JomashopNotifications.Persistence.Entities;

public sealed record ProductImageEntity
{
    public bool IsPrimary { get; set; }
    public required byte[] Data { get; set; }
}
