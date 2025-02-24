using JomashopNotifications.Domain.Common;
using JomashopNotifications.Domain.Models;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JomashopNotifications.Domain;

// Option for Edge/Google driver selection - appsettings.json
// IAsyncEnumerable? semaphore?
public sealed class JomashopBrowserDriverService(IOptions<ChromeOptions> chromeOptions)
{
    private readonly ChromeOptions _chromeOptions = chromeOptions.Value;

    public async Task<Either<Product.Enriched, BrowserDriverError>> FetchProductDataAsync(Uri link)
    {
        using var chromeService = ResolveChromeDriverService();

        using var driver = new ChromeDriver(chromeService, _chromeOptions);

        var htmlOrError = await NavigateAndGetHtmlAsync(driver, link);

        return htmlOrError.Match(
                html => Either<Product.Enriched, BrowserDriverError>.Left(
                            new Product.ToEnrich(link)
                                       .ParseFromHtml(html)),
                Either<Product.Enriched, BrowserDriverError>.Right);
    }

    public async Task<List<Either<Product.Checked, (int id, BrowserDriverError error)>>> CheckProductsAsync(
        IEnumerable<Product.ToBeChecked> productsToCheck)
    {
        using var chromeService = ResolveChromeDriverService();

        using var driver = new ChromeDriver(chromeService, _chromeOptions);

        var results = new List<Either<Product.Checked, (int, BrowserDriverError)>>();

        foreach (var productToCheck in productsToCheck)
        {
            var result = await CheckProductAsync(productToCheck);
            results.Add(result);
        }

        driver.Quit();

        return results;

        async Task<Either<Product.Checked, (int id, BrowserDriverError error)>> CheckProductAsync(Product.ToBeChecked productToCheck)
        {
            var htmlOrError = await NavigateAndGetHtmlAsync(driver, productToCheck.Link);

            return htmlOrError.Match(
                    html => Either<Product.Checked, (int, BrowserDriverError)>.Left(
                                productToCheck.ParseFromHtml(driver.PageSource)),
                    error => Either<Product.Checked, (int, BrowserDriverError)>.Right(
                                (productToCheck.Id, error)));
        }
    }

    private static async Task<Either<string, BrowserDriverError>> NavigateAndGetHtmlAsync(ChromeDriver driver, Uri link)
    {
        try
        {
            await driver.Navigate()
                        .GoToUrlAsync(link);

            WaitForPageLoad(driver);

            await WaitToMimicHumanBehavior();

            return Either<string, BrowserDriverError>.Left(driver.PageSource);
        }
        catch (Exception ex)
        {
            return Either<string, BrowserDriverError>.Right(new(ex));
        }
    }

    private static ChromeDriverService ResolveChromeDriverService()
    {
        var service = ChromeDriverService.CreateDefaultService();

        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        return service;
    }

    private static void WaitForPageLoad(IWebDriver driver, int timeout = 5) =>
        new WebDriverWait(driver, TimeSpan.FromSeconds(timeout))
                .Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")
                                                    .Equals("complete"));

    private static Task WaitToMimicHumanBehavior() =>
        Task.Delay(new Random().Next(2000, 3000));
}