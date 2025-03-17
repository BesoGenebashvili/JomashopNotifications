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
        var productFullName = $"{context.Message.Brand} {context.Message.Name}";

        var subject = $"Jomashop Notification: {productFullName} is now in stock!";

        var primaryImageBase64 = context.Message.GetPrimaryImageBase64();

        var body = $@"""
                     <p><strong>Great news!</strong> The product you're interested in '{productFullName}' is now available.</p>
                     <p><img src='data:image/jpeg;base64,{primaryImageBase64}' alt='Product Image' width='300' /></p>
                     <p><a href='{context.Message.Link}'>🔗 View Product</a></p>
                     """;

        await emailService.SendAsync(subject, body, true);
    }
}
