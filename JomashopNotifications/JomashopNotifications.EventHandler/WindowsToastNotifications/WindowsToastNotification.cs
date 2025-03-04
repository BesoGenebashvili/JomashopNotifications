using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;

namespace JomashopNotifications.EventHandler.WindowsToastNotifications;

public sealed class WindowsToastNotification
{
    public static void RegisterEventSubscribers()
    {
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

            if (!args.TryGetValue(WindowsToastKey.Action, out string action) && action is not WindowsToastArgument.Open)
                return;

            var productId = args.Get(WindowsToastKey.ProductId);
            var link = args.Get(WindowsToastKey.Link);

            Console.WriteLine($"Inside action: {action}, activated for {productId}, link: {link}");

            OpenLinkInBrowser(link);
        };

        static void OpenLinkInBrowser(string link)
        {
            if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
            {
                Console.WriteLine($"Invalid link: {link}");
            }

            Process.Start(new ProcessStartInfo(link)
            {
                UseShellExecute = true
            });
        }
    }
}
