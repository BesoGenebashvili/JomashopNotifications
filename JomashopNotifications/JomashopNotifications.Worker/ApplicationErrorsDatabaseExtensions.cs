using Serilog;
using JomashopNotifications.Persistence.Abstractions;

namespace JomashopNotifications.Worker;

public static class ApplicationErrorsDatabaseExtensions
{
    public static async Task LogAsync(
        this IApplicationErrorsDatabase self,
        Exception exception)
    {
        try
        {
            await self.InsertAsync(
                exception.ToString(),
                exception.GetType()
                         .ToString());
        }
        catch (Exception ex)
        {
            Log.Fatal("An error occurred while logging an error in the application errors database. Error: {ex}", ex);
        }
    }
}
