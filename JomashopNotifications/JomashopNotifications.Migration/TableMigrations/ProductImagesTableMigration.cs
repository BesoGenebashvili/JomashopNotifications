using FluentMigrator;
using JomashopNotifications.Persistence.Common;
using System.Data;

namespace JomashopNotifications.Migration.TableMigrations;

[Migration(7)]
public class ProductImagesTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.ProductImages)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey(DatabaseTable.Products, "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("IsPrimary").AsBoolean()
                                      .NotNullable()
              .WithColumn("ImageData").AsBinary()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.ProductImages);
}