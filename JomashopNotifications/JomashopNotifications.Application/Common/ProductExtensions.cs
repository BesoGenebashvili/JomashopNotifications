using JomashopNotifications.Application.InStockProduct.Contracts;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Persistence.Entities;

namespace JomashopNotifications.Application.Common;

public static class ProductExtensions
{
    public static ProductDto ToDto(this ProductEntity self) =>
        new(self.Id,
            self.Brand,
            self.Name,
            self.Link,
            self.Status,
            self.CreatedAt.ToLocalTime(),
            self.UpdatedAt.ToLocalTime());

    public static InStockProductDto ToDto(this InStockProductEntity self) =>
        new(self.Id,
            self.ProductId,
            self.Price,
            self.CheckedAt.ToLocalTime());
}
