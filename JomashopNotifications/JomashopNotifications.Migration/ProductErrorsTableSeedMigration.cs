using FluentMigrator;

namespace JomashopNotifications.Migration;

[Migration(5)]
public sealed class ProductErrorsTableSeedMigration : FluentMigrator.Migration
{
    const string TableName = "ProductErrors";

    public override void Up() =>
        Create.Table(TableName)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey("Products", "Id")
              .WithColumn("Message").AsString()
                                    .NotNullable()
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(TableName);
}