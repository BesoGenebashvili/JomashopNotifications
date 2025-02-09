using Microsoft.Extensions.DependencyInjection;

namespace JomashopNotifications.Application;

public static class ApplicationAssemblyReference { }

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatR(this IServiceCollection services) =>
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly));
}
