using JomashopNotifications.Application.InStockProduct.Contracts;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Application.ProductProfile.Contracts;
using JomashopNotifications.Domain.Models;
using JomashopNotifications.Persistence.Entities.Product;
using JomashopNotifications.Persistence.Entities.ProductProfile;

namespace JomashopNotifications.Application.Common;

public static class ProductExtensions
{
    private static ProductImageDto ToDto(this ProductImageEntity self) =>
        new(self.IsPrimary,
            self.ImageData);

    public static ProductDto ToDto(this ProductEntity self) =>
        new(self.Id,
            self.Brand,
            self.Name,
            self.Link,
            self.Status,
            self.CreatedAt.ToLocalTime(),
            self.UpdatedAt.ToLocalTime(),
            self.Images.Select(i => i.ToDto())
                       .ToList());

    public static InStockProductDto ToDto(this InStockProductEntity self) =>
        new(self.Id,
            self.ProductId,
            new(self.Price, 
                Enum.Parse<Currency>(self.Currency)),
            self.CheckedAt.ToLocalTime());

    public static ProductProfileDto ToDto(this ProductProfileEntity self) =>
        new(self.ProductId,
            self.PriceThreshold,
            self.IsActive,
            self.CreatedAt);
}
