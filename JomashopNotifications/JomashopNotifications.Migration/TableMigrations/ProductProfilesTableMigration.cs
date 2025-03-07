using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration.TableMigrations;

// Currency?
[Migration(9)]
public sealed class ProductProfilesTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.ProductProfiles)
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .PrimaryKey()
                                      .ForeignKey(DatabaseTable.Products, "Id")
              .WithColumn("IsActive").AsBoolean()
                                     .NotNullable()
                                     .WithDefaultValue(true)
              .WithColumn("PriceThreshold").AsDecimal()
                                           .NotNullable()
              .WithColumn("CreatedAt").AsDateTime2()
                                      .NotNullable()
                                      .WithDefault(SystemMethods.CurrentUTCDateTime);
    public override void Down() =>
        Delete.Table(DatabaseTable.ProductProfiles);
}
