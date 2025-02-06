using HtmlAgilityPack;

public abstract record Item(Uri Link)
{
    public sealed record InStock(Uri Link, Money Price) : Item(Link);
    public sealed record OutOfStock(Uri Link) : Item(Link);
    public sealed record Error(Uri Link, string Message) : Item(Link);

    public override string ToString() => this switch
    {
        InStock(var link, var price) => $"In stock, Link: {link.AbsoluteUri.AsBrief(40)}, Price: {price.Value}{price.Currency.AsSymbol()}",
        OutOfStock(var link) => $"Out of stock, Link: {link.AbsoluteUri.AsBrief(40)}",
        Error(var link) => $"Error, Link: {link.AbsoluteUri.AsBrief(40)}",
        _ => throw new NotImplementedException(nameof(Item))
    };

    public static Item ParseFromHtml(Uri link, string html)
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
                "IN_STOCK" => new InStock(link, GetPrice(htmlDocument) ?? throw new InvalidOperationException("item does not contain a price")),
                "OUT_OF_STOCK" => new OutOfStock(link),
                null => throw new InvalidOperationException($"{StockAttributeName} does not contain a value"),
                _ => throw new InvalidOperationException($"{StockAttributeName} contains an invalid value")
            };
        }
        catch (Exception ex)
        {
            return new Error(link, ex.Message);
        }

        static Money? GetPrice(HtmlDocument htmlDocument)
        {
            const string PriceElementClass = "now-price";

            var priceText = htmlDocument.DocumentNode
                                        .SelectSingleNode($"//div[@class='{PriceElementClass}']")
                                        ?.ChildNodes
                                        ?.FirstOrDefault()
                                        ?.InnerText ?? throw new ArgumentException("Price text does not contain value");

            return Money.Parse(priceText);
        }
    }
}