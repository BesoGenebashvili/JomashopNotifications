using JomashopNotifications.Application.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JomashopNotifications.EventHandler.EmailNotifications;

public sealed class ProductInStockEventEmailNotificationHandler(
    ILogger<ProductInStockEventEmailNotificationHandler> logger) : IConsumer<ProductInStockEvent>
{
    public async Task Consume(ConsumeContext<ProductInStockEvent> context)
    {

    }
}
