using FluentMigrator;
using JomashopNotifications.Persistence.Common;
using System.Collections.Immutable;

namespace JomashopNotifications.Migration.TableSeedMigrations;

[Migration(8)]
public class ProductImagesTableSeedMigration : FluentMigrator.Migration
{
    private static readonly string imageFolderPath = 
        Path.Combine(Directory.GetCurrentDirectory(), "Images");

    private static byte[] ReadImage(string fileName) =>
        File.ReadAllBytes(Path.Combine(imageFolderPath, fileName));

    private ImmutableList<(int productId, bool isPrimary, byte[] imageData)> ResolveRecords() =>
        [
            // Orient
            (1, true, ReadImage("orient - 1 (primary).jpg")),
            (1, false, ReadImage("orient - 2.jpg")),
            (1, false, ReadImage("orient - 3.jpg")),

            // IWC
            (2, true, ReadImage("iwc - 1 (primary).jpg")),
            (2, false, ReadImage("iwc - 2.jpg")),
            (2, false, ReadImage("iwc - 3.jpg")),
        ];

    public override void Up() =>
        ResolveRecords().ForEach(
            item => Insert.IntoTable(DatabaseTable.ProductImages)
                          .Row(new
                          {
                              ProductId = item.productId,
                              IsPrimary = item.isPrimary,
                              ImageData = item.imageData
                          }));

    public override void Down() =>
        Delete.FromTable(DatabaseTable.ProductImages)
              .AllRows();
}
