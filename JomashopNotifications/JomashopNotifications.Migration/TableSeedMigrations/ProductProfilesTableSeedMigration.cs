using FluentMigrator;
using JomashopNotifications.Persistence.Common;
using System.Collections.Immutable;

namespace JomashopNotifications.Migration.TableSeedMigrations;

[Migration(2025_03_07_11_00)]
public class ProductProfilesTableSeedMigration : FluentMigrator.Migration
{
    private static ImmutableList<(int productId, bool isActive, decimal priceThreshold)> ResolveRecords =>
        [
            (1, true, 200),
            (2, false, 3900),
            (3, true, 4400),
            (5, false, 1500)
        ];

    public override void Up() =>
        ResolveRecords.ForEach(
            item => Insert.IntoTable(DatabaseTable.ProductProfiles)
                          .Row(new
                          {
                              ProductId = item.productId,
                              IsActive = item.isActive,
                              PriceThreshold = item.priceThreshold
                          }));

    public override void Down() =>
        Delete.FromTable(DatabaseTable.ProductProfiles)
              .AllRows();
}
