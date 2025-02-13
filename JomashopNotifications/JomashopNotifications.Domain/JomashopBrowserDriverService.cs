using JomashopNotifications.Domain.Common;
using JomashopNotifications.Domain.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JomashopNotifications.Domain;

public sealed class JomashopBrowserDriverService
{
    // Option for Edge/Google driver selection - appsettings.json
    // IAsyncEnumerable? semaphore?
    public async Task<List<Either<Product.Checked, BrowserDriverError>>> CheckProductsAsync(
        IEnumerable<Product.ToBeChecked> productsToCheck)
    {
        using var chromeService = ResolveChromeDriverService();
        var chromeOptions = ResolveChromeOptions();

        using var driver = new ChromeDriver(chromeService, chromeOptions);

        var results = new List<Either<Product.Checked, BrowserDriverError>>();

        foreach (var productToCheck in productsToCheck)
        {
            var result = await CheckProductAsync(productToCheck);
            results.Add(result);
        }

        driver.Close();

        return results;

        ChromeDriverService ResolveChromeDriverService()
        {
            var service = ChromeDriverService.CreateDefaultService();

            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            return service;
        }

        ChromeOptions ResolveChromeOptions()
        {
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArguments(
                "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36", // To mimic real browser
                "--host-resolver-rules=MAP ec2-52-23-111-175.compute-1.amazonaws.com 127.0.0.1",
                "--headless", // Without UI
                "--incognito", // Private
                "--log-level=3"); // Without logs

            return chromeOptions;
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
        Task.Delay(new Random().Next(2000, 5000));
}