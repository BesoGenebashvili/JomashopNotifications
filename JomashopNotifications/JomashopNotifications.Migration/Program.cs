using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator;
using FluentMigrator.Runner;

// From Appsettings
const string ConnectionString = "Server=DESKTOP-5R95BQP;Database=Test;Integrated Security=True;TrustServerCertificate=True";

using var serviceProvider = CreateServiceProvider(ConnectionString);
using var serviceScope = serviceProvider.CreateScope();

var migrationRunner = serviceScope.ServiceProvider.GetRequiredService<IMigrationRunner>();

// From Appsettings
migrationRunner.MigrateUp();

Console.WriteLine("Hello, World!");

static ServiceProvider CreateServiceProvider(string connectionString) =>
    new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(builder =>
            builder.AddSqlServer()
                   .WithGlobalConnectionString(connectionString)
                   .ScanIn(typeof(Program).Assembly).For.Migrations())
        .AddLogging(builder => builder.AddFluentMigratorConsole())
        .BuildServiceProvider(false);

[Migration(1)]
public class JomashopLinksTableMigration : Migration
{
    const string TableName = "JomashopLinks";

    public override void Up()
    {
        Create.Table(TableName)
              .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
              .WithColumn("Link").AsString().NotNullable().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}

[Profile("")]
public sealed class JomashopLinksTableSeedMigration : Migration
{
    const string TableName = "JomashopLinks";

    private readonly ImmutableList<Uri> _links =
        [
            new("https://www.jomashop.com/orient-contemporary-classic-champagne-dial-mens-watch-ra-ac0m04y10b.html"),
            new("https://www.jomashop.com/oris-big-crown-automatic-white-dial-watch-01-754-7741-4081-set.html"),
            new("https://www.jomashop.com/iwc-watch-iw325519.html"),
            new("https://www.jomashop.com/omega-watch-220-10-38-20-02-003.html"),
            new("https://www.jomashop.com/longines-heritage-automatic-silver-dial-mens-watch-l2-828-4-73-2.html"),
            new("https://www.jomashop.com/tissot-tissot-heritage-pink-dial-unisex-watch-t1424641633200.html"),
            new("https://www.jomashop.com/ball-engineer-ii-automatic-black-dial-mens-watch-nm1020c-s4-bk.html"),
            new("https://www.jomashop.com/a-lange-and-sohne-watch-140-029.html"),
            new("https://www.jomashop.com/vacheron-constantin-6000v-110a-b544.html")
        ];

    public override void Up()
    {
        _links.Select((uri, id) => (Id: ++id, Link: uri.AbsoluteUri))
              .ToList()
              .ForEach(item => ExecuteInsert(item.Id, item.Link));

        void ExecuteInsert(int id, string link) =>
            Insert.IntoTable(TableName)
                  .Row(new
                  {
                      Id = id,
                      Link = link
                  });
    }

    public override void Down() =>
        Delete.FromTable(TableName)
              .AllRows();
}
