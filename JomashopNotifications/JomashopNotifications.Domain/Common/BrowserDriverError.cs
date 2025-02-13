namespace JomashopNotifications.Domain.Common;

public record BrowserDriverError(int ProductId, string Message);

public static class BrowserDriverErrorExtensions
{
    public static BrowserDriverError FromException(this Exception self, int productId) =>
        new(productId, self.Message);
}