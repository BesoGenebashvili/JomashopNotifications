using JomashopNotifications.Domain;
using JomashopNotifications.Application;
using JomashopNotifications.Persistence;

namespace JomashopNotifications.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDomainServices()
                .AddApplicationServices()
                .AddPersistenceServices(configuration)
                .AddEndpointsApiExplorer()
                .AddSwaggerGen();
}
