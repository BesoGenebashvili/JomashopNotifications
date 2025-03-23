using Microsoft.AspNetCore.HttpLogging;

namespace JomashopNotifications.Api;

public static class SwaggerExtensions
{
    public static IEndpointRouteBuilder MapSwaggerRoutes(this IEndpointRouteBuilder self)
    {
        self.MapGet("/", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription()
            .WithHttpLogging(HttpLoggingFields.None);

        self.MapGet("/index.html", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription()
            .WithHttpLogging(HttpLoggingFields.None);

        self.MapGet("/swagger/v1/swagger.json", () => Results.Ok())
            .ExcludeFromDescription()
            .WithHttpLogging(HttpLoggingFields.None);

        self.MapGet("/swagger/index.js", () => Results.Ok())
            .ExcludeFromDescription()
            .WithHttpLogging(HttpLoggingFields.None);

        self.MapGet("/swagger/index.html", () => Results.Ok())
            .ExcludeFromDescription()
            .WithHttpLogging(HttpLoggingFields.None);

        return self;
    }
}
