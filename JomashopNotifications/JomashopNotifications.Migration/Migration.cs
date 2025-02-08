using FluentMigrator.Runner;
using Microsoft.Extensions.Options;

internal sealed class Migration
{
    private readonly IMigrationRunner _migrationRunner;
    private readonly AppSettings _appSettings;

    public Migration(IMigrationRunner migrationRunner, IOptions<AppSettings> appSettingsOptions)
    {
        _migrationRunner = migrationRunner;
        _appSettings = appSettingsOptions.Value;
    }

    public void Run()
    {
        if (_appSettings.CreateDatabaseTables)
        {
            _migrationRunner.MigrateUp();
        }
        else if (_appSettings.DeleteDatabaseTables)
        {
            var version = 0;
            _migrationRunner.MigrateDown(version);
        }
    }
}