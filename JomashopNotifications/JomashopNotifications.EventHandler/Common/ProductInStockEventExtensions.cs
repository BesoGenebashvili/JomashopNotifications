using JomashopNotifications.Application.Messages;

namespace JomashopNotifications.EventHandler.Common;

public static class ProductInStockEventExtensions
{
    public static byte[]? GetPrimaryImage(this ProductInStockEvent self) =>
        self.Images
            .FirstOrDefault(x => x.IsPrimary)?
            .ImageData;

    public static string? GetPrimaryImageBase64(this ProductInStockEvent self) =>
        self.GetPrimaryImage() is { Length: > 0 } primaryImage
            ? Convert.ToBase64String(primaryImage)
            : null;
}
