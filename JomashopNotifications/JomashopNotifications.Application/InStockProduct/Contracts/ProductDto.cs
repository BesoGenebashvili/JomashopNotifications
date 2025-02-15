namespace JomashopNotifications.Application.InStockProduct.Contracts;

public sealed record InStockProductDto(
    int Id,
    int ProductId,
    decimal Price,
    DateTime CheckedAt);
