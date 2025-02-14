namespace JomashopNotifications.Persistence.Entities;

public sealed record InsertProductEntity
{
    public required string Link { get; init; }
    public required ProductStatus Status { get; init; }
}
