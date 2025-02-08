using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;

var configuration = CreateConfiguration();
using var serviceProvider = CreateServiceProvider(configuration);
using var serviceScope = serviceProvider.CreateScope();

var migration = serviceScope.ServiceProvider.GetRequiredService<Migration>();

migration.Run();

Console.WriteLine("Hello, World!");

static IConfiguration CreateConfiguration() =>
    new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

static ServiceProvider CreateServiceProvider(IConfiguration configuration) =>
    new ServiceCollection()
        .AddSingleton(configuration)
        .Configure<AppSettings>(configuration.GetSection("AppSettings"))
        .AddFluentMigratorCore()
        .Configure<RunnerOptions>(options => options.Profile = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"))
        .ConfigureRunner(builder =>
            builder.AddSqlServer()
                   .WithGlobalConnectionString(configuration.GetConnectionString("DefaultConnection"))
                   .ScanIn(typeof(Program).Assembly).For.Migrations())
        .AddLogging(builder => builder.AddFluentMigratorConsole())
        .AddTransient<Migration>()
        .BuildServiceProvider(false);

internal sealed record AppSettings
{
    public bool CreateDatabaseTables { get; init; }
    public bool DeleteDatabaseTables { get; init; }
}
