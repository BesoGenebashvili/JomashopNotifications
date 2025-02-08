using System.Data;
using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration;

[Migration(3)]
public sealed class InStockProductsTableSeedMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.InStockProducts)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey("Products", "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("Price").AsDecimal()
                                  .NotNullable()
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.InStockProducts);
}
