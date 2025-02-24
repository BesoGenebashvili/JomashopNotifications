namespace JomashopNotifications.Persistence.Entities;

public sealed record InsertProductEntity
{
    public required string Brand { get; init; }
    public required string Name { get; set; }
    public required string Link { get; init; }
    public required ProductStatus Status { get; init; }
}
