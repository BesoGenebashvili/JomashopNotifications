﻿using FluentMigrator;
using System.Collections.Immutable;

namespace JomashopNotifications.Migration;

[Profile("Development")]
public class ProductLinksTableSeedMigration : FluentMigrator.Migration
{
    const string TableName = "ProductLinks";

    private readonly ImmutableList<Uri> _links =
        [
            new("https://www.jomashop.com/orient-contemporary-classic-champagne-dial-mens-watch-ra-ac0m04y10b.html"),
            new("https://www.jomashop.com/oris-big-crown-automatic-white-dial-watch-01-754-7741-4081-set.html"),
            new("https://www.jomashop.com/iwc-watch-iw325519.html"),
            new("https://www.jomashop.com/omega-watch-220-10-38-20-02-003.html"),
            new("https://www.jomashop.com/longines-heritage-automatic-silver-dial-mens-watch-l2-828-4-73-2.html"),
            new("https://www.jomashop.com/tissot-tissot-heritage-pink-dial-unisex-watch-t1424641633200.html"),
            new("https://www.jomashop.com/ball-engineer-ii-automatic-black-dial-mens-watch-nm1020c-s4-bk.html"),
            new("https://www.jomashop.com/a-lange-and-sohne-watch-140-029.html"),
            new("https://www.jomashop.com/vacheron-constantin-6000v-110a-b544.html")
        ];

    public override void Up() =>
        _links.ForEach(
            link => Insert.IntoTable(TableName)
                          .Row(new
                          {
                              Link = link.AbsoluteUri
                          }));

    public override void Down() =>
        Delete.FromTable(TableName)
              .AllRows();
}
