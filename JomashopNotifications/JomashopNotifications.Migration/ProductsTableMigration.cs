using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration;

[Migration(1)]
public class ProductsTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.Products)
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
        Delete.Table(DatabaseTable.Products);
}