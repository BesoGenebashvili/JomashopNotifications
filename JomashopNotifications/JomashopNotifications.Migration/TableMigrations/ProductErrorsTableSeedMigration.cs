﻿using System.Data;
using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration.TableMigrations;

[Migration(5)]
public sealed class ProductErrorsTableSeedMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.ProductErrors)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey("Products", "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("Message").AsString()
                                    .NotNullable()
              .WithColumn("CheckedAt").AsDateTime2()
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.ProductErrors);
}