using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Application.Product.Contracts;

public record ProductDto(
    int Id,
    string Link,
    ProductStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
