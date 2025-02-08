using FluentMigrator;

namespace JomashopNotifications.Migration;

[Migration(3)]
public sealed class InStockProductsTableSeedMigration : FluentMigrator.Migration
{
    const string TableName = "InStockProducts";

    public override void Up() =>
        Create.Table(TableName)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey("Products", "Id")
              .WithColumn("Price").AsDecimal()
                                  .NotNullable()
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(TableName);
}
