using FluentMigrator;

namespace JomashopNotifications.Migration;

[Migration(1)]
public class ProductTableMigration : FluentMigrator.Migration
{
    const string TableName = "Product";

    public override void Up() =>
        Create.Table(TableName)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("Link").AsString()
                                 .NotNullable()
                                 .Unique()
              .WithColumn("Status").AsInt32()
                                   .NotNullable()
              .WithColumn("CreatedAt").AsDateTime2()
                                      .NotNullable()
                                      .WithDefault(SystemMethods.CurrentUTCDateTime)
              .WithColumn("UpdatedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(TableName);
}