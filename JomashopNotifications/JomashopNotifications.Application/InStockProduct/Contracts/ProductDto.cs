namespace JomashopNotifications.Application.InStockProduct.Contracts;

public sealed record InStockProductDto(
    int Id,
    int ProductId,
    MoneyDto Price,
    DateTime CheckedAt);
