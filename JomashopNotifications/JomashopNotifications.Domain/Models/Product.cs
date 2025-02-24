using HtmlAgilityPack;
using JomashopNotifications.Domain.Common;

namespace JomashopNotifications.Domain.Models;

public abstract record Product(Uri Link)
{
    public sealed record ToBeChecked(int Id, string Brand, string Name, Uri Link) : Product(Link);

    public abstract record Checked(ToBeChecked Reference, DateTime CheckedAt) : Product(Reference.Link)
    {
        public sealed record InStock(ToBeChecked Reference, Money Price, DateTime CheckedAt) : Checked(Reference, CheckedAt);
        public sealed record OutOfStock(ToBeChecked Reference, DateTime CheckedAt) : Checked(Reference, CheckedAt);

        // Change this to ParseError
        public sealed record Error(ToBeChecked Reference, string Message, DateTime CheckedAt) : Checked(Reference, CheckedAt);
    }

    public string Show() => this switch
    {
        ToBeChecked(var id, var brand, var name, { AbsoluteUri: var link }) =>
            $"[To be checked] Id: {id}, Brand: {brand}, Name: {name}, Link: {link.AsBrief()}",

        Checked.InStock({ Link.AbsoluteUri: var link }, var price, var checkedAt) =>
            $"[In stock] CheckedAt: {checkedAt:MM-dd HH:mm:ss}, Link: {link.AsBrief()}, Price: {price.Value}{price.Currency.AsSymbol()}",

        Checked.OutOfStock({ Link.AbsoluteUri: var link }, var checkedAt) =>
            $"[Out of stock] CheckedAt: {checkedAt:MM-dd HH:mm:ss}, Link: {link.AsBrief()}",

        Checked.Error({ Link.AbsoluteUri: var link }, var message, var checkedAt) =>
            $"[Error] CheckedAt: {checkedAt:MM-dd HH:mm:ss}, Message: {message}, Link: {link.AsBrief()}",

        _ => throw new NotImplementedException(nameof(Product))
    };
}

public static class ProductExtensions
{
    public static Product.Checked.InStock InStock(this Product.ToBeChecked self, Money price, DateTime checkedAt) => new(self, price, checkedAt);
    public static Product.Checked.OutOfStock OutOfStock(this Product.ToBeChecked self, DateTime checkedAt) => new(self, checkedAt);
    public static Product.Checked.Error Error(this Product.ToBeChecked self, string message, DateTime checkedAt) => new(self, message, checkedAt);

    // Error ?
    public static (string Brand, string Name) ParseFromHtml(string html)
    {
        const string BrandElementClass = "brand-name";
        const string NameElementClass = "product-name";

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

        return (brand, name);
    }

    public static Product.Checked ParseFromHtml(
        this Product.ToBeChecked self,
        string html)
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
            return self.Error(ex.Message, now);
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