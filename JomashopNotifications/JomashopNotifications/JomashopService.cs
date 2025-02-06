using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

public static class JomashopService
{
    public static async IAsyncEnumerable<Either<Item, BrowserDriverError>> ParseItemsFromLinksAsync(IEnumerable<Uri> uris)
    {
        var service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments(
            "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36",
            "--host-resolver-rules=MAP ec2-52-23-111-175.compute-1.amazonaws.com 127.0.0.1",
            "--headless",
            "--incognito",
            "--log-level=3");

        using var driver = new ChromeDriver(service, chromeOptions);

        foreach (var uri in uris)
        {
            var errorOrHtml = await NavigateAndGetHtml(uri);

            yield return errorOrHtml.Match(
                html => Either<Item, BrowserDriverError>.Left(
                            Item.ParseFromHtml(
                                    uri,
                                    driver.PageSource)),
                Either<Item, BrowserDriverError>.Right);
        }

        driver.Quit();

        async Task<Either<string, BrowserDriverError>> NavigateAndGetHtml(Uri uri)
        {
            try
            {
                await driver.Navigate()
                            .GoToUrlAsync(uri);

                WaitForPageLoad(driver);
                await WaitToMimicHumanBehavior();

                return Either<string, BrowserDriverError>.Left(driver.PageSource);
            }
            catch (Exception ex)
            {
                return Either<string, BrowserDriverError>.Right(ex.FromException());
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