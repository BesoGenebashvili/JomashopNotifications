using MassTransit;
using JomashopNotifications.Application.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using JomashopNotifications.Domain.Models;
using JomashopNotifications.Application.InStockProduct.Contracts;

namespace JomashopNotifications.EventHandler.WindowsToastNotifications;

public sealed class WindowsToastNotificationHandler(ILogger<WindowsToastNotificationHandler> logger) : IConsumer<ProductInStockEvent>
{
    public Task Consume(ConsumeContext<ProductInStockEvent> context)
    {
        // TODO: Update Windows toast notification to handle 'OnActivated' event

        var message = context.Message;

        var primaryImage = message.ProductImages
                                  .FirstOrDefault(i => i.IsPrimary)?
                                  .ImageData;

        ShowToastNotification(
            message.Brand,
            message.Name,
            primaryImage,
            context.Message.Price,
            context.Message.CheckedAt);


        logger.LogInformation("Received product in stock event {@Message}", context.Message);

        return Task.CompletedTask;
    }

    public static async void ShowToastNotification(
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
                      : WriteImageAndGetPath();

        var toastNotification =
            new ToastContentBuilder()
                .AddText($"{brand} {name} is in stock for {price.Amount}{price.Currency.AsSymbol()} !")
                .AddInlineImage(new Uri(imagePath))
                .AddAttributionText($"Checked at {checkedAt:M, HH:dd:ss}")
                .AddButton(new ToastButtonDismiss())
                .SetToastDuration(ToastDuration.Long)
                .SetToastScenario(ToastScenario.Default);

        toastNotification.Show();

        if (!imagePath.Contains("default-watch.png"))
        {
            // Temporary image should be deleted after a few seconds to prevent unnecessary file accumulation
            await Task.Delay(5000);
            File.Delete(imagePath);
        }

        string WriteImageAndGetPath()
        {
            var temporaryPath = Path.Combine(imageFolderPath, $"{Guid.NewGuid()}.jpg");

            File.WriteAllBytes(temporaryPath, primaryImage);

            return temporaryPath;
        }
    }
}
