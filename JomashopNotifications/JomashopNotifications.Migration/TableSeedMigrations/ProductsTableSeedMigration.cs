using FluentMigrator;
using JomashopNotifications.Persistence.Common;
using JomashopNotifications.Persistence.Entities;
using System.Collections.Immutable;

namespace JomashopNotifications.Migration.TableSeedMigrations;

[Migration(2)]
public class ProductsTableSeedMigration : FluentMigrator.Migration
{
    private readonly ImmutableList<(string brand, string name, string link)> records =
        [
            ("Orient", "Contemporary Classic Champagne Dial Men's Watch", "https://www.jomashop.com/orient-contemporary-classic-champagne-dial-mens-watch-ra-ac0m04y10b.html"),
            ("IWC", "Big Pilot Son Watch Silver Dial Black Leather Men's Watch", "https://www.jomashop.com/iwc-watch-iw325519.html"),
            ("Omega", "Seamaster Aqua Terra Automatic Chronometer Silver Dial Ladies Watch", "https://www.jomashop.com/omega-watch-220-10-38-20-02-003.html"),
            ("Longines", "Heritage Automatic Silver Dial Men's Watch", "https://www.jomashop.com/longines-heritage-automatic-silver-dial-mens-watch-l2-828-4-73-2.html"),
            ("Tissot", "Heritage Automatic Pink Dial Men's Watch", "https://www.jomashop.com/tissot-tissot-heritage-pink-dial-unisex-watch-t1424641633200.html"),
            ("Ball", "Engineer II Automatic Black Dial Men's Watch", "https://www.jomashop.com/ball-engineer-ii-automatic-black-dial-mens-watch-nm1020c-s4-bk.html"),
            ("A. Lange & Sohne", "A. Lange and Sohne Zeitwerk Black Dial 18K White Gold Men's Watch", "https://www.jomashop.com/a-lange-and-sohne-watch-140-029.html"),
            ("Vacheron Constantin", "Overseas Tourbillon Automatic Blue Dial Men's Watch", "https://www.jomashop.com/vacheron-constantin-6000v-110a-b544.html")
        ];

    public override void Up() =>
        records.ForEach(
            item => Insert.IntoTable(DatabaseTable.Products)
                          .Row(new
                          {
                              Brand = item.brand,
                              Name = item.name,
                              Link = item.link,
                              Status = (int)ProductStatus.Active,
                              UpdatedAt = DateTime.UtcNow
                          }));

    public override void Down() =>
        Delete.FromTable(DatabaseTable.Products)
              .AllRows();
}
