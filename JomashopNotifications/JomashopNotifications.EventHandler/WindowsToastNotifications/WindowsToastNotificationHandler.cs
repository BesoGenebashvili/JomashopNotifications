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
        // TODO: Update Windows toast notification

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

    // CCY?
    public static void ShowToastNotification(
        string brand,
        string name,
        byte[]? primaryImage,
        MoneyDto price,
        DateTime checkedAt)
    {
        const string DefaultImagePath = @"WindowsToastNotifications\Images\default-watch.png";

        var inlineImageUri = 
            new Uri(
                primaryImage is not null
                    ? ConvertImageToUri(primaryImage)
                    : Path.Combine(Environment.CurrentDirectory, DefaultImagePath));

        var toastNotification =
            new ToastContentBuilder()
                .AddText($"{brand} {name} is in stock for {price.Amount}{price.Currency.AsSymbol()} !")
                .AddInlineImage(inlineImageUri)
                .AddAttributionText($"Checked at {checkedAt:M, HH:dd:ss}")
                .AddButton(new ToastButtonDismiss())
                .SetToastDuration(ToastDuration.Long)
                .SetToastScenario(ToastScenario.Reminder);

        toastNotification.Show();

        static string ConvertImageToUri(byte[] imageData) =>
            $"data:image/jpeg;base64,{Convert.ToBase64String(imageData)}";
    }
}
