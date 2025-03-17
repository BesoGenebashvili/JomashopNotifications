using JomashopNotifications.EventHandler.EmailNotifications;

namespace JomashopNotifications.EventHandler;

public sealed record ProductInStockEventEmailNotificationHandlerOptions(
    bool IsActive, 
    EmailOptions EmailOptions);

public sealed record ProductInStockEventToastNotificationHandlerOptions(
    bool IsActive);

public sealed record EventHandlerOptions
{
    public const string SectionName = "EventHandlerOptions";

    public required ProductInStockEventEmailNotificationHandlerOptions ProductInStockEventEmailNotificationHandlerOptions { get; init; }
    public required ProductInStockEventToastNotificationHandlerOptions ProductInStockEventToastNotificationHandlerOptions { get; init; }
}
