using JomashopNotifications.Domain.Common;
using JomashopNotifications.Domain.Models;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JomashopNotifications.Domain;

public sealed class JomashopBrowserDriverService(IOptions<ChromeOptions> chromeOptions)
{
    private readonly ChromeOptions _chromeOptions = chromeOptions.Value;

    // Option for Edge/Google driver selection - appsettings.json
    // IAsyncEnumerable? semaphore?
    public async Task<List<Either<Product.Checked, BrowserDriverError>>> CheckProductsAsync(
        IEnumerable<Product.ToBeChecked> productsToCheck)
    {
        using var chromeService = ResolveChromeDriverService();

        using var driver = new ChromeDriver(chromeService, _chromeOptions);

        var results = new List<Either<Product.Checked, BrowserDriverError>>();

        foreach (var productToCheck in productsToCheck)
        {
            var result = await CheckProductAsync(productToCheck);
            results.Add(result);
        }

        driver.Quit();

        return results;

        ChromeDriverService ResolveChromeDriverService()
        {
            var service = ChromeDriverService.CreateDefaultService();

            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            return service;
        }

        async Task<Either<Product.Checked, BrowserDriverError>> CheckProductAsync(Product.ToBeChecked productToCheck)
        {
            var htmlOrError = await NavigateAndGetHtml(productToCheck);

            return htmlOrError.Match(
                     html => Either<Product.Checked, BrowserDriverError>.Left(
                                 productToCheck.ParseFromHtml(driver.PageSource)),
                     Either<Product.Checked, BrowserDriverError>.Right);
        }

        async Task<Either<string, BrowserDriverError>> NavigateAndGetHtml(Product.ToBeChecked productToCheck)
        {
            try
            {
                await driver.Navigate()
                            .GoToUrlAsync(productToCheck.Link);

                WaitForPageLoad(driver);

                await WaitToMimicHumanBehavior();

                return Either<string, BrowserDriverError>.Left(driver.PageSource);
            }
            catch (Exception ex)
            {
                return Either<string, BrowserDriverError>.Right(ex.FromException(productToCheck.Id));
            }
        }
    }

    private static void WaitForPageLoad(IWebDriver driver, int timeout = 5) =>
        new WebDriverWait(driver, TimeSpan.FromSeconds(timeout))
                .Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")
                                                    .Equals("complete"));

    private static Task WaitToMimicHumanBehavior() =>
        Task.Delay(new Random().Next(2000, 3000));
}