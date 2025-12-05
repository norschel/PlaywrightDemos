using Microsoft.Playwright;

namespace PlayDemo;

//[TestClass] - ignore it
#pragma warning disable MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
public class PlaywrightE2ETests_MDD2025
#pragma warning restore MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
{
    #region Globals
    static bool _isHeadless = true;
    #endregion

    #region SimpleSmokeTest
    [TestMethod]
    public async Task MDD_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless,
                SlowMo = 2000,
            });
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://www.md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Sessions").First.ClickAsync();
        //await page.GetByRole(AriaRole.Tab, new() { Name = "14.05." }).ClickAsync();
        
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();

        var sessionLink = page.Locator(".act-card-content-container").
            Filter(new() { HasText = "(12.5.) Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.PauseAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.Locator("text=Magdeburg").IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region DataDrivenSmokeTest
    [TestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    [DataRow("Edge")]
    [DataRow("Chrome")]
    public async Task MDD_DataDrivenSmokeTest(string BrowserName)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await GetBrowserAsync(playwright, BrowserName);
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://www.md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Sessions").First.ClickAsync();
        //await page.GetByRole(AriaRole.Tab, new() { Name = "14.05." }).ClickAsync();
        await page.Locator("id=mat-tab-label-0-0").ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        var sessionLink = page.Locator(".act-card-content-container").
            Filter(new() { HasText = "(12.5.) Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.Locator("text=Magdeburg").IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }

    private static async Task<IBrowser> GetBrowserAsync(IPlaywright playwright, string BrowserName)
    {
        var browserOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
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
            case "Edge":
                browserOptions.Channel = "msedge";
                return await playwright.Chromium.LaunchAsync(browserOptions);
            case "Chrome":
                browserOptions.Channel = "chrome";
                return await playwright.Chromium.LaunchAsync(browserOptions);
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
                Headless = _isHeadless,
                SlowMo = 2000
            });
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://mediadaten.heise.de/en/home/rate-cards/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "heise_ratecards.png" });

        var task = page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.Locator("text=ct_2025").ClickAsync();
        });

        await task.Result.SaveAsAsync("mediadaten_ct_2024.pdf");

        Assert.IsTrue(File.Exists("mediadaten_ct_2024.pdf"));

        await browser.CloseAsync();

    }

    [TestMethod]
    public async Task SevenZip_PlaywrightDownloadTest_()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless,
                SlowMo = 2000
            });
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await page.GotoAsync("https://www.7-zip.org/");

        await page.ScreenshotAsync(
            new PageScreenshotOptions
            {
                Path = "SevenZipPage.png",
            });

        var task = page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.Locator("[href*=x64]")
            .And(page.GetByRole(AriaRole.Link))
            .ClickAsync();
        });

        await task.Result.SaveAsAsync("7zip-x64.exe");
        Assert.IsTrue(File.Exists("7zip-x64.exe"));
        await browser.CloseAsync();
    }
    #endregion

    #region DeviceTest
    [TestMethod]
    public async Task MDD_DeviceTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless,
                SlowMo = 2000
            });

        // execute test on iPhone 13 landscape
        var device = playwright.Devices["iPhone 13 landscape"];
        var browserContext = await browser.NewContextAsync(device);
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://www.md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Sessions").First.ClickAsync();
        //await page.GetByRole(AriaRole.Tab, new() { Name = "14.05." }).ClickAsync();
        await page.Locator("id=mat-tab-label-0-0").ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        var sessionLink = page.Locator(".act-card-content-container").
            Filter(new() { HasText = "(12.5.) Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        //await page.PauseAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.Locator("text=Magdeburg").IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region VideoTest
    [TestMethod]
    public async Task MDD_VideoSimpleTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless,
                SlowMo = 2000
            });

        var browserContextOptions = new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/",
            RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 }
        };
        var browserContext = await browser.NewContextAsync(browserContextOptions);

        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://www.md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Sessions").First.ClickAsync();
        //await page.GetByRole(AriaRole.Tab, new() { Name = "14.05." }).ClickAsync();
        await page.Locator("id=mat-tab-label-0-0").ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        var sessionLink = page.Locator(".act-card-content-container").
            Filter(new() { HasText = "(12.5.) Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.Locator("text=Magdeburg").IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion
}
