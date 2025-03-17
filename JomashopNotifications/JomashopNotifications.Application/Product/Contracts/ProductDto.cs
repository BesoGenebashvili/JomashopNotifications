using JomashopNotifications.Persistence.Entities.Product;

namespace JomashopNotifications.Application.Product.Contracts;

public sealed record ProductDto(
    int Id,
    string Brand,
    string Name,
    string Link,
    ProductStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ProductImageDto> Images);
