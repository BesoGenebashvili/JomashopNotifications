namespace JomashopNotifications.Application.ProductProfile.Contracts;

public sealed record ProductProfileDto(
    int ProductId,
    decimal PriceThreshold,
    bool IsActive,
    DateTime CreatedAt);