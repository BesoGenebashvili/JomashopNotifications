using FluentMigrator.Runner;
using Microsoft.Extensions.Options;

internal sealed class Migration(
    IMigrationRunner migrationRunner, 
    IOptions<AppSettings> appSettingsOptions)
{
    private readonly AppSettings appSettings = appSettingsOptions.Value;

    public void Run()
    {
        if (appSettings.CreateDatabaseTables)
        {
            migrationRunner.MigrateUp();
        }
        else if (appSettings.DeleteDatabaseTables)
        {
            // From settings
            var version = 0;

            migrationRunner.MigrateDown(version);
        }
    }
}