using System.Data;
using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration.TableMigrations;

// Currency?
[Migration(9)]
public sealed class ProductProfilesTableMigration : FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table(DatabaseTable.ProductProfiles)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey(DatabaseTable.Products, "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("IsActive").AsBoolean()
                                     .NotNullable()
                                     .WithDefaultValue(true)
              .WithColumn("PriceThreshold").AsDecimal()
                                           .NotNullable()
              .WithColumn("CreatedAt").AsDateTime2()
                                      .NotNullable()
                                      .WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.ForeignKey("FK_ProductProfiles_Products")
              .FromTable("ProductProfiles").ForeignColumn("ProductId")
              .ToTable("Products").PrimaryColumn("Id")
              .OnDelete(Rule.Cascade);

        Create.UniqueConstraint("UQ_ProductProfiles_ProductId")
              .OnTable("ProductProfiles")
              .Column("ProductId");
    }
    public override void Down() =>
        Delete.Table(DatabaseTable.ProductProfiles);
}
