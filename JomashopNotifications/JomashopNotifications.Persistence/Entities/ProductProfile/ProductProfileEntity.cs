namespace JomashopNotifications.Persistence.Entities.ProductProfile;

public sealed record ProductProfileEntity
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required decimal PriceThreshold { get; init; }
    public required bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
