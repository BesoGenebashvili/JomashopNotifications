using FluentMigrator;

namespace JomashopNotifications.Migration;

[Migration(4)]
public sealed class OutOfStockProductsTableSeedMigration : FluentMigrator.Migration
{
    const string TableName = "OutOfStockProducts";

    public override void Up() =>
        Create.Table(TableName)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey("Products", "Id")
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(TableName);
}
