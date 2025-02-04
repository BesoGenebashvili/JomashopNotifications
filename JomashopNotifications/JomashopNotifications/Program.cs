using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

Console.WriteLine("Hello, World!");

var links = GetJomashopLinks();
var items = ParseItemsFromLinks(links);

foreach (var item in items)
{
    Console.WriteLine(item);
}

static IEnumerable<Uri> GetJomashopLinks()
{
    yield return new Uri("https://www.jomashop.com/orient-contemporary-classic-champagne-dial-mens-watch-ra-ac0m04y10b.html");
    yield return new Uri("https://www.jomashop.com/oris-big-crown-automatic-white-dial-watch-01-754-7741-4081-set.html");
    yield return new Uri("https://www.jomashop.com/iwc-watch-iw325519.html");
    yield return new Uri("https://www.jomashop.com/omega-watch-220-10-38-20-02-003.html");
    yield return new Uri("https://www.jomashop.com/longines-heritage-automatic-silver-dial-mens-watch-l2-828-4-73-2.html");
    yield return new Uri("https://www.jomashop.com/tissot-tissot-heritage-pink-dial-unisex-watch-t1424641633200.html");
    yield return new Uri("https://www.jomashop.com/ball-engineer-ii-automatic-black-dial-mens-watch-nm1020c-s4-bk.html");
    yield return new Uri("https://www.jomashop.com/a-lange-and-sohne-watch-140-029.html");
    yield return new Uri("https://www.jomashop.com/vacheron-constantin-6000v-110a-b544.html");
}

static IEnumerable<Item> ParseItemsFromLinks(IEnumerable<Uri> uris)
{
    var chromeOptions = new ChromeOptions();
    chromeOptions.AddArgument("-headless");
    using var driver = new ChromeDriver(chromeOptions);

    foreach (var uri in uris)
    {
        driver.Navigate()
              .GoToUrl(uri);

        WaitForPageLoad(driver);

        yield return Item.ParseFromHtml(
                            uri,
                            driver.PageSource);
    }

    driver.Quit();
}

static void WaitForPageLoad(IWebDriver driver, int timeoutSec = 5) =>
    new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec))
            .Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")
                                                .Equals("complete"));


public static class StringExtensions
{
    public static string AsBrief(
        this string self,
        int characterCount,
        string suffix = "...") =>
        self.Length <= characterCount
            ? self
            : self[..characterCount] + suffix;
}

public static class CurrencyExtensions
{
    public static char AsSymbol(this Currency self) => self switch
    {
        Currency.USD => '$',
        Currency.EUR => '€',
        _ => throw new NotImplementedException(nameof(Currency))
    };
}

public abstract record Item(Uri Link)
{
    public record InStock(Uri Link, Money Price) : Item(Link);
    public record OutOfStock(Uri Link) : Item(Link);
    public record Error(Uri Link, string Message) : Item(Link);

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

public enum Currency : byte
{
    USD,
    EUR
}

public record Money(decimal Value, Currency Currency)
{
    public static Money Parse(string value) =>
        TryParse(value, out var result)
            ? result!
            : throw new FormatException($"Can't resolve Money from: {value}");

    public static bool TryParse(string value, out Money? result)
    {
        result = value switch
        {
        ['$', .. var n] => new(decimal.Parse(n, CultureInfo.InvariantCulture), Currency.USD),
        ['€', .. var n] => new(decimal.Parse(n, CultureInfo.InvariantCulture), Currency.EUR),
            _ => null,
        };

        return result != null;
    }
}