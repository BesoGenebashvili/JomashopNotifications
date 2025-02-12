using HtmlAgilityPack;
using JomashopNotifications.Domain.Common;

namespace JomashopNotifications.Domain.Models;

public abstract record Product(Uri Link)
{
    public sealed record ToBeChecked(Uri Link) : Product(Link);

    public abstract record Checked(ToBeChecked Reference) : Product(Reference.Link)
    {
        public sealed record InStock(ToBeChecked Reference, Money Price) : Checked(Reference);
        public sealed record OutOfStock(ToBeChecked Reference) : Checked(Reference);
        public sealed record Error(ToBeChecked Reference, string Message) : Checked(Reference);
    }

    public string Show() => this switch
    {
        ToBeChecked(var link) => $"To be checked, Link: {link.AbsoluteUri.AsBrief()}",
        Checked.InStock(var reference, var price) => $"In stock, Link: {reference.Link.AbsoluteUri.AsBrief()}, Price: {price.Value}{price.Currency.AsSymbol()}",
        Checked.OutOfStock(var reference) => $"Out of stock, Link: {reference.Link.AbsoluteUri.AsBrief()}",
        Checked.Error(var reference) => $"Error, Link: {reference.Link.AbsoluteUri.AsBrief()}",
        _ => throw new NotImplementedException(nameof(Product))
    };
}

public static class ProductExtensions
{
    public static Product.Checked.InStock InStock(this Product.ToBeChecked self, Money price) => new(self, price);
    public static Product.Checked.OutOfStock OutOfStock(this Product.ToBeChecked self) => new(self);
    public static Product.Checked.Error Error(this Product.ToBeChecked self, string message) => new(self, message);

    // CheckedTime ->
    public static Product.Checked ParseFromHtml(
        this Product.ToBeChecked self,
        string html)
    {
        const string StockAttributeName = "data-preload-product-stock-status";

        try
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var metaElement = htmlDocument.DocumentNode
                                          .SelectSingleNode($"//meta[@{StockAttributeName}]")
                                          ?? throw new ArgumentException($"{StockAttributeName} was not found");

            return metaElement?.GetAttributeValue(StockAttributeName, null) switch
            {
                "IN_STOCK" => self.InStock(GetPrice(htmlDocument)),
                "OUT_OF_STOCK" => self.OutOfStock(),
                null => throw new InvalidOperationException($"{StockAttributeName} does not contain a value"),
                _ => throw new InvalidOperationException($"{StockAttributeName} contains an invalid value")
            };
        }
        catch (Exception ex)
        {
            return self.Error(ex.Message);
        }

        static Money GetPrice(HtmlDocument htmlDocument) =>
            TryGetPrice(htmlDocument) ?? throw new InvalidOperationException("item does not contain a price");

        static Money? TryGetPrice(HtmlDocument htmlDocument)
        {
            const string PriceElementClass = "now-price";

            var priceText = htmlDocument.DocumentNode
                                        .SelectSingleNode($"//div[@class='{PriceElementClass}']")
                                        ?.ChildNodes
                                        ?.FirstOrDefault()
                                        ?.InnerText ?? throw new ArgumentException("Price text does not contain a value");

            return Money.Parse(priceText);
        }
    }
}