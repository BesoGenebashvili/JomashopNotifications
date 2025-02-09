using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Application.Common;

public static class ProductExtensions
{
    public static ProductDto ToDto(this ProductEntity productEntity) =>
        new(productEntity.Id,
            productEntity.Link,
            productEntity.Status,
            productEntity.CreatedAt.ToLocalTime(),
            productEntity.UpdatedAt.ToLocalTime());
}
