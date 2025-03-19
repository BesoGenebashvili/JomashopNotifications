using JomashopNotifications.Application.Messages;
using JomashopNotifications.EventHandler.EmailNotifications;
using System.Globalization;

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

    public static async Task<Dictionary<EmailTemplatePlaceholderKey, string>> FlattenAsync(this ProductInStockEvent self)
    {
        return new()
        {
            [EmailTemplatePlaceholderKey.Brand] = self.Brand,
            [EmailTemplatePlaceholderKey.Name] = self.Name,
            [EmailTemplatePlaceholderKey.Price] = $"{self.Price.Amount.ToString(CultureInfo.InvariantCulture)} {self.Price.Currency}",
            [EmailTemplatePlaceholderKey.Link] = self.Link,
            [EmailTemplatePlaceholderKey.FullName] = $"{self.Brand} {self.Name}",
            [EmailTemplatePlaceholderKey.CheckedAt] = self.CheckedAt.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt"),
            [EmailTemplatePlaceholderKey.PrimaryImageBase64] = self.GetPrimaryImageBase64() ?? await GetDefaultImageAsync()
        };

        static async Task<string> GetDefaultImageAsync() =>
            Convert.ToBase64String(
                await File.ReadAllBytesAsync(
                    Path.Combine(
                        Environment.CurrentDirectory,
                        "Common",
                        "Images",
                        "default-watch.png")));
    }
}
