﻿using FluentMigrator;
using JomashopNotifications.Persistence.Common;

namespace JomashopNotifications.Migration.TableMigrations;

[Migration(2025_02_07_01_00)]
public class ApplicationErrorsTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.ApplicationErrors)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("Message").AsString()
                                    .NotNullable()
                                    .Unique()
              .WithColumn("Type").AsInt32()
              .WithColumn("CreatedAt").AsDateTime2()
                                      .NotNullable()
                                      .WithDefault(SystemMethods.CurrentUTCDateTime);

    public override void Down() =>
        Delete.Table(DatabaseTable.ApplicationErrors);
}