using FluentMigrator;

namespace JomashopNotifications.Migration;

[Migration(1)]
public class ProductLinksTableMigration : FluentMigrator.Migration
{
    const string TableName = "ProductLinks";

    public override void Up() =>
        Create.Table(TableName)
              .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
              .WithColumn("Link").AsString().NotNullable().Unique();

    public override void Down() =>
        Delete.Table(TableName);
}