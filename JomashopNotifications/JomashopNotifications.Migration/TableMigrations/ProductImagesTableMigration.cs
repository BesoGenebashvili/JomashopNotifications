﻿using FluentMigrator;
using JomashopNotifications.Persistence.Common;
using System.Data;

namespace JomashopNotifications.Migration.TableMigrations;

[Migration(2025_02_08_01_00)]
public class ProductImagesTableMigration : FluentMigrator.Migration
{
    public override void Up() =>
        Create.Table(DatabaseTable.ProductImages)
              .WithColumn("Id").AsInt32()
                               .NotNullable()
                               .PrimaryKey()
                               .Identity()
              .WithColumn("ProductId").AsInt32()
                                      .NotNullable()
                                      .ForeignKey(DatabaseTable.Products, "Id")
                                      .OnDelete(Rule.Cascade)
              .WithColumn("IsPrimary").AsBoolean()
                                      .NotNullable()
              .WithColumn("ImageData").AsBinary(int.MaxValue)
                                      .NotNullable();

    public override void Down() =>
        Delete.Table(DatabaseTable.ProductImages);
}