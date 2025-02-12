using JomashopNotifications.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace JomashopNotifications.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services) =>
        services.JomashopBrowserDriverService();

    private static IServiceCollection JomashopBrowserDriverService(this IServiceCollection services) =>
        services.AddTransient<JomashopBrowserDriverService>();
}
