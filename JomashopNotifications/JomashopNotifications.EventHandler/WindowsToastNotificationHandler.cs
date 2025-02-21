using MassTransit;
using JomashopNotifications.Application.Messages;
using Microsoft.Extensions.Logging;

namespace JomashopNotifications.EventHandler;

public sealed class WindowsToastNotificationHandler(ILogger<WindowsToastNotificationHandler> logger) : IConsumer<ProductInStockEvent>
{
    public Task Consume(ConsumeContext<ProductInStockEvent> context)
    {
        // TODO: Implement Windows toast notification

        logger.LogInformation("Received product in stock event {@Message}", context.Message);

        return Task.CompletedTask;
    }
}
