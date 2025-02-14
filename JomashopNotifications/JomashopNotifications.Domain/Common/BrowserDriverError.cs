namespace JomashopNotifications.Domain.Common;

public record BrowserDriverError(
    Exception Exception,
    int ProductId)
{
    public string Message => Exception.Message;
}

public static class BrowserDriverErrorExtensions
{
    public static BrowserDriverError FromException(this Exception self, int productId) =>
        new(self, productId);
}