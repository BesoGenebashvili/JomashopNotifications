using JomashopNotifications.Persistence.Abstractions;
using JomashopNotifications.Persistence.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JomashopNotifications.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("Missing ConnectionStrings.DefaultConnection in appsettings.json");

        return services.AddSingleton<IProductsDatabase>(_ => new ProductsSqlDatabase(connectionString));
    }
}
