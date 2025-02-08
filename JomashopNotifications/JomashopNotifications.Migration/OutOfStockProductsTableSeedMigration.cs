using System.Data;
using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration;

[Migration(4)]
public sealed class OutOfStockProductsTableSeedMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.OutOfStockProducts)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey("Products", "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.OutOfStockProducts);
}
