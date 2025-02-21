using Microsoft.Extensions.Configuration;

namespace JomashopNotifications.Application.Common;

public static class ConfigurationExtensions
{
    // App.Common ?
    public static TOptions GetOptionsOrFail<TOptions>(this IConfiguration configuration, string sectionName) =>
        configuration.GetSection(sectionName)
                     .Get<TOptions>()
                     ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
}
