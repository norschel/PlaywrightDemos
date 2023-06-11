using Microsoft.Playwright;

namespace PlayDemo;

[TestClass]
public class PlaywrightE2ETests
{
    #region SimpleSmokeTest
    [TestMethod]
    public async Task Entwicklertag_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 2000
            });
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://entwicklertag.de/");
        await page.Locator("text=Programm").First.ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        await page.Locator("text=Playwright").ClickAsync();
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        var title = await page.TitleAsync();
        Assert.IsTrue(title.Contains("Playwright"));
        await page.Locator("text=Rheinauen").IsVisibleAsync();

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region DataDrivenSmokeTest
    [DataTestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    public async Task Entwicklertag_DataDrivenSmokeTest(string BrowserName)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await GetBrowserAsync(playwright, BrowserName);
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://entwicklertag.de/");
        await page.Locator("text=Programm").First.HighlightAsync();
        await page.Locator("text=Programm").First.ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        await page.Locator("text=Playwright").ClickAsync();
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        var title = await page.TitleAsync();
        Assert.IsTrue(title.Contains("Playwright"));
        await page.Locator("text=Rheinauen").IsVisibleAsync();

        //await page.PauseAsync();
        await browser.CloseAsync();
    }

    private static async Task<IBrowser> GetBrowserAsync(IPlaywright playwright, string BrowserName)
    {
        var browserOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 2000
        };

        switch (BrowserName)
        {
            case "Chromium":
                return await playwright.Chromium.LaunchAsync(browserOptions);
            case "Firefox":
                return await playwright.Firefox.LaunchAsync(browserOptions);
            case "Webkit":
                return await playwright.Webkit.LaunchAsync(browserOptions);
            default:
                throw new ArgumentException("Browser not supported");
        }
    }
    #endregion

    #region DowmloadTest
    [TestMethod]
    public async Task HeiseMediadaten_PlaywrightDownloadTest_()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 2000
            });
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://mediadaten.heise.de/en/home/rate-cards/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "heise_ratecards.png" });

        var task = page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.Locator("text=ct_2023").ClickAsync();
        });

        await task.Result.SaveAsAsync("mediadaten_ct_2023.pdf");

        Assert.IsTrue(File.Exists("mediadaten_ct_2023.pdf"));

        await browser.CloseAsync();

    }
    #endregion

    #region DeviceTest
    [TestMethod]
    public async Task Entwicklertag_DeviceTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 2000
            });

        // execute test on iPhone 13 landscape
        var device = playwright.Devices["iPhone 13 landscape"];
        var browserContext = await browser.NewContextAsync(device);

        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://entwicklertag.de/");
        await page.Locator("text=Programm").First.ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        await page.Locator("text=Playwright").ClickAsync();
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        var title = await page.TitleAsync();
        Assert.IsTrue(title.Contains("Playwright"));
        await page.Locator("text=Rheinauen").IsVisibleAsync();

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region VideoTest
    [TestMethod]
    public async Task Entwicklertag_VideoSimpleTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                //Headless = false,
                SlowMo = 2000
            });

        var browserContextOptions = new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/",
            RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 }
        };
        var browserContext = await browser.NewContextAsync(browserContextOptions);

        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://entwicklertag.de/");
        await page.Locator("text=Programm").First.ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        await page.Locator("text=Playwright").ClickAsync();
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        var title = await page.TitleAsync();
        Assert.IsTrue(title.Contains("Playwright"));
        await page.Locator("text=Rheinauen").IsVisibleAsync();

        await browserContext.CloseAsync();
        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion
}