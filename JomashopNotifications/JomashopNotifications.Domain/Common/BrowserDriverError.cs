namespace JomashopNotifications.Domain.Common;

public record BrowserDriverError(Exception Exception)
{
    public string Message => Exception.Message;
}