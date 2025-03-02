using System.Data;
using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration.TableMigrations;

[Migration(3)]
public sealed class InStockProductsTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.InStockProducts)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey(DatabaseTable.Products, "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("Price").AsDecimal()
                                  .NotNullable()
              .WithColumn("Currency").AsString(3)
                                     .NotNullable()
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.InStockProducts);
}
