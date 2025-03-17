using MassTransit;
using JomashopNotifications.Application.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using JomashopNotifications.Domain.Models;
using JomashopNotifications.Application.InStockProduct.Contracts;
using JomashopNotifications.EventHandler.Common;

namespace JomashopNotifications.EventHandler.WindowsToastNotifications;

public sealed class ProductInStockEventToastNotificationHandler(
    ILogger<ProductInStockEventToastNotificationHandler> logger) : IConsumer<ProductInStockEvent>
{
    public async Task Consume(ConsumeContext<ProductInStockEvent> context)
    {
        var message = context.Message;

        logger.LogInformation("Received 'ProductInStockEvent' for product: {ProductId}, Message: {@Message}", message.ProductId, message);

        var primaryImage = message.GetPrimaryImage();

        await ShowToastNotificationAsync(
            message.ProductId,
            message.Link,
            message.Brand,
            message.Name,
            primaryImage,
            message.Price,
            message.CheckedAt);
    }

    public static async Task ShowToastNotificationAsync(
        int productId,
        string link,
        string brand,
        string name,
        byte[]? primaryImage,
        MoneyDto price,
        DateTime checkedAt)
    {
        var imageFolderPath = Path.Combine(
            Environment.CurrentDirectory,
            "WindowsToastNotifications",
            "Images");

        var imagePath = primaryImage is null
                      ? Path.Combine(imageFolderPath, "default-watch.png")
                      : StoreImageAndGetPath();

        var toastNotification =
            new ToastContentBuilder()
                .AddArgument(WindowsToastKey.ProductId, productId)
                .AddText($"{brand} {name} is in stock for {price.Amount}{price.Currency.AsSymbol()} !")
                .AddInlineImage(new Uri(imagePath))
                .AddAttributionText($"Checked at {checkedAt:MMMM M, HH:dd:ss}")
                .AddButton(new ToastButton()
                    .SetContent("Open")
                    .AddArgument(WindowsToastKey.Action, WindowsToastArgument.Open)
                    .AddArgument(WindowsToastKey.Link, link)
                    .SetBackgroundActivation())
                .AddButton(new ToastButtonDismiss())
                .SetToastDuration(ToastDuration.Long)
                .SetToastScenario(ToastScenario.Default);

        toastNotification.Show();

        if (!imagePath.Contains("default-watch.png"))
        {
            // Temporary image should be deleted after a few seconds to prevent unnecessary file accumulation
            // Unfortunately I can't pass binary data as a URI to AddInlineImage
            await Task.Delay(5000);
            File.Delete(imagePath);
        }

        string StoreImageAndGetPath()
        {
            var temporaryPath = Path.Combine(imageFolderPath, $"{Guid.NewGuid()}.jpg");

            File.WriteAllBytes(temporaryPath, primaryImage);

            return temporaryPath;
        }
    }
}
