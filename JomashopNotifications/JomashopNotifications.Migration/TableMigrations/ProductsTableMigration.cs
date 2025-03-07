using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration.TableMigrations;

[Migration(2025_02_07_00_00)]
public class ProductsTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.Products)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("Brand").AsString()
                                  .NotNullable()
              .WithColumn("Name").AsString()
                                 .NotNullable()
              .WithColumn("Link").AsString()
                                 .NotNullable()
                                 .Unique()
              .WithColumn("Status").AsInt32()
                                   .NotNullable()
              .WithColumn("CreatedAt").AsDateTime2()
                                      .NotNullable()
                                      .WithDefault(SystemMethods.CurrentUTCDateTime)
              .WithColumn("UpdatedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.Products);
}