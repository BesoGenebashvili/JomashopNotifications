using JomashopNotifications.Application.Messages;
using JomashopNotifications.EventHandler.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JomashopNotifications.EventHandler.EmailNotifications;

public sealed class ProductInStockEventEmailNotificationHandler(
    EmailService emailService,
    ILogger<ProductInStockEventEmailNotificationHandler> logger) : IConsumer<ProductInStockEvent>
{
    public async Task Consume(ConsumeContext<ProductInStockEvent> context)
    {
        logger.LogInformation(
            "Received 'ProductInStockEvent' in 'ProductInStockEventEmailNotificationHandler' for product: {ProductId}, Message: {@Message}", 
            context.Message.ProductId, 
            context.Message);

        await emailService.SendAsync(await context.Message.FlattenAsync());
    }
}
