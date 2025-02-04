using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

Console.WriteLine("Hello, World!");

var url = new Uri("https://www.jomashop.com/orient-contemporary-classic-champagne-dial-mens-watch-ra-ac0m04y10b.html");

var chromeOptions = new ChromeOptions();
chromeOptions.AddArgument("-headless");
var driver = new ChromeDriver(chromeOptions);

driver.Navigate()
      .GoToUrl(url);

WaitForPageLoad(driver);

var pageHtml = driver.PageSource;

driver.Quit();

static void WaitForPageLoad(IWebDriver driver, int timeoutSec = 5) =>
    new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec))
            .Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")
                                                .Equals("complete"));

public abstract record Item(Uri Link)
{
    public record InStock(Uri Link, Money Price) : Item(Link);
    public record OutOfStock(Uri Link) : Item(Link);
    public record Error(Uri Link, string Message) : Item(Link);

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