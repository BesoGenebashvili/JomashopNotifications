﻿#pragma warning disable IDE0055

using HtmlAgilityPack;
using JomashopNotifications.Domain.Common;
using System.Diagnostics;

namespace JomashopNotifications.Domain.Models;

// In Product?
public sealed record ProductImage(
    bool IsPrimary,
    Uri ImageLink)
{
    public static ProductImage Primary(Uri imageLink) => new(true, imageLink);
    public static ProductImage Secondary(Uri imageLink) => new(false, imageLink);
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract record Product(Uri Link)
{
    public sealed record ToEnrich(Uri Link) : Product(Link);

    public abstract record Enriched(Uri Link) : Product(Link)
    {
        public sealed record Success(ToEnrich Reference, string Brand, string Name, IReadOnlyList<ProductImage> Images) : Enriched(Reference.Link);
        public sealed record ParseError(ToEnrich Reference, string Message) : Enriched(Reference.Link);
    }

    public sealed record ToCheck(int Id, Uri Link) : Product(Link);

    public abstract record Checked(ToCheck Reference, DateTime CheckedAt) : Product(Reference.Link)
    {
        public sealed record InStock(ToCheck Reference, Money Price, DateTime CheckedAt) : Checked(Reference, CheckedAt);
        public sealed record OutOfStock(ToCheck Reference, DateTime CheckedAt) : Checked(Reference, CheckedAt);
        public sealed record ParseError(ToCheck Reference, string Message, DateTime CheckedAt) : Checked(Reference, CheckedAt);
    }

    public string Show() => this switch
    {
        ToEnrich({ AbsoluteUri: var link }) =>
            $"[To enrich] Link: {link.AsBrief()}",

        Enriched.Success({ Link.AbsoluteUri: var link }, var brand, var name, { Count: var imageCount }) =>
            $"[Successfully enriched] Brand: {brand}, Name: {name}, Image count: {imageCount}, Link: {link.AsBrief()}",

        Enriched.ParseError({ Link.AbsoluteUri: var link }, var message) =>
            $"[Error while enriching] Message: {message}, Link: {link.AsBrief()}",

        ToCheck(var id, { AbsoluteUri: var link }) =>
            $"[To check] Id: {id}, Link: {link.AsBrief()}",

        Checked.InStock({ Link.AbsoluteUri: var link }, var price, var checkedAt) =>
            $"[In stock] CheckedAt: {checkedAt:MM-dd HH:mm:ss}, Link: {link.AsBrief()}, Price: {price.Value}{price.Currency.AsSymbol()}",

        Checked.OutOfStock({ Link.AbsoluteUri: var link }, var checkedAt) =>
            $"[Out of stock] CheckedAt: {checkedAt:MM-dd HH:mm:ss}, Link: {link.AsBrief()}",

        Checked.ParseError({ Link.AbsoluteUri: var link }, var message, var checkedAt) =>
            $"[Error] CheckedAt: {checkedAt:MM-dd HH:mm:ss}, Message: {message}, Link: {link.AsBrief()}",

        _ => throw new NotImplementedException(nameof(Product))
    };

    private string DebuggerDisplay => Show();
}

public static class ProductExtensions
{
    public static Product.Checked.InStock InStock(this Product.ToCheck self, Money price, DateTime checkedAt) => new(self, price, checkedAt);
    public static Product.Checked.OutOfStock OutOfStock(this Product.ToCheck self, DateTime checkedAt) => new(self, checkedAt);
    public static Product.Checked.ParseError ParseError(this Product.ToCheck self, string message, DateTime checkedAt) => new(self, message, checkedAt);

    public static Product.Enriched.ParseError ParseError(this Product.ToEnrich self, string message) => new(self, message);
    public static Product.Enriched.Success Success(
        this Product.ToEnrich self,
        string brand,
        string name,
        IReadOnlyList<ProductImage> images) => new(self, brand, name, images);

    public static Product.Enriched ParseFromHtml(this Product.ToEnrich self, string html)
    {
        const string BrandElementClass = "brand-name";
        const string NameElementClass = "product-name";
        const string ImageGalleryElementClass = "image-main__gallery";
        const string ImageAttributeName = "src";

        try
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var brand = htmlDocument.DocumentNode
                                    .SelectSingleNode($"//span[@class='{BrandElementClass}']")
                                    .InnerText
                                    .Trim(' ', '"');

            var name = htmlDocument.DocumentNode
                                   .SelectSingleNode($"//span[@class='{NameElementClass}']")
                                   .InnerText
                                   .Trim(' ', '"');

            var imageUris = htmlDocument.DocumentNode
                                        .SelectSingleNode($"//div[@class='{ImageGalleryElementClass}']")?
                                        .SelectNodes(".//img")?
                                        .Select(n => n.GetAttributeValue(ImageAttributeName, null))
                                        .Where(a => a is not null && Uri.IsWellFormedUriString(a, UriKind.Absolute))
                                        .Select(u => new Uri(u))
                                        .ToArray();

            ProductImage[] images = imageUris switch
            {
                [var primary] => [ProductImage.Primary(primary)],
                [var primary, .. var secondaries] => [ProductImage.Primary(primary), .. secondaries.Select(ProductImage.Secondary)],
                _ => []
            };

            return self.Success(brand, name, images);
        }
        catch (Exception ex)
        {
            return self.ParseError(ex.Message);
        }
    }

    public static Product.Checked ParseFromHtml(this Product.ToCheck self, string html)
    {
        const string StockAttributeName = "data-preload-product-stock-status";
        var now = DateTime.UtcNow;

        try
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var metaElement = htmlDocument.DocumentNode
                                          .SelectSingleNode($"//meta[@{StockAttributeName}]")
                                          ?? throw new ArgumentException($"{StockAttributeName} was not found");


            return metaElement?.GetAttributeValue(StockAttributeName, null) switch
            {
                "IN_STOCK" => self.InStock(GetPrice(htmlDocument), now),
                "OUT_OF_STOCK" => self.OutOfStock(now),
                null => throw new InvalidOperationException($"{StockAttributeName} does not contain a value"),
                _ => throw new InvalidOperationException($"{StockAttributeName} contains an invalid value")
            };
        }
        catch (Exception ex)
        {
            return self.ParseError(ex.Message, now);
        }

        static Money GetPrice(HtmlDocument htmlDocument) =>
            TryGetPrice(htmlDocument) ?? throw new InvalidOperationException("item does not contain a price");

        static Money? TryGetPrice(HtmlDocument htmlDocument)
        {
            const string PriceElementClass = "now-price";

            var priceText = htmlDocument.DocumentNode
                                        .SelectSingleNode($"//div[@class='{PriceElementClass}']")?
                                        .ChildNodes?
                                        .FirstOrDefault()?
                                        .InnerText;

            return priceText switch
            {
                { Length: > 0 } when Money.TryParse(priceText, out var value) => value,
                _ => null
            };
        }
    }
}