namespace JomashopNotifications.Application.Product.Contracts;

public sealed record ProductImageDto(
    bool IsPrimary,
    byte[] ImageData);
