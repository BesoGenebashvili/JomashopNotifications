using JomashopNotifications.Domain;
using JomashopNotifications.Application;
using JomashopNotifications.Persistence;
using Microsoft.AspNetCore.HttpLogging;
using System.Text.Json.Serialization;

namespace JomashopNotifications.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDomainServices()
                .AddApplicationServices()
                .AddPersistenceServices(configuration)
                .AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddHttpLogging()
                .AddControllers()
                .AddJsonOptions();

    private static IServiceCollection AddHttpLogging(this IServiceCollection self) =>
        self.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPath
                                  | HttpLoggingFields.RequestHeaders
                                  | HttpLoggingFields.RequestBody
                                  | HttpLoggingFields.ResponseHeaders
                                  | HttpLoggingFields.ResponseBody;

            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;

            options.CombineLogs = true;
        });

    /// <summary>
    /// Adds Enum values as strings in the response/request
    /// </summary>
    private static IServiceCollection AddJsonOptions(this IMvcBuilder self) =>
        self.AddJsonOptions(options =>
                options.JsonSerializerOptions
                       .Converters
                       .Add(new JsonStringEnumConverter()))
            .Services;
}
