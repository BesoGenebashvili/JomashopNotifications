using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Chrome;

namespace JomashopNotifications.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services) =>
        services.AddChromeOptions()
                .AddJomashopBrowserDriverService();

    private static IServiceCollection AddJomashopBrowserDriverService(this IServiceCollection services) =>
        services.AddTransient<JomashopBrowserDriverService>();

    public static IServiceCollection AddChromeOptions(this IServiceCollection services) =>
        services.Configure<ChromeOptions>(chromeOptions =>
            chromeOptions.AddArguments(
                "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36", // To mimic real browser
                "--host-resolver-rules=MAP ec2-52-23-111-175.compute-1.amazonaws.com 127.0.0.1",
                "--headless", // Without UI
                "--incognito", // Private
                "--log-level=3")); // Without logs
}
