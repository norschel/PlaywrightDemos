using Microsoft.Playwright;

namespace demo123;

//[TestClass] - Disabled for now
public class DDC2023_Demos
{
    //playwright.Selectors.SetTestIdAttribute("id");

    //var tag2Label = page.GetByTestId("tag-2-label");
    //await tag2Label.ClickAsync();

    #region SimpleSmokeTest
    [TestMethod]
    public async Task DDC2023_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false
            ,SlowMo = 2000
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync("https://www.dotnet-developer-conference.de/");
        await page.GetByTestId("uc-accept-all-button").ClickAsync();
        await page.ClickAsync("text=Programm");
        await page.ClickAsync("id=tag-2-label");
        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_wdc2023.png" });

        Assert.IsTrue(
            page.GetByRole(
                AriaRole.Heading,
                new() { Name = "Testautomatisierung für WebApps mit Playwright" })
            .IsVisibleAsync().Result);
        await browser.CloseAsync();
    }
    #endregion

    #region Video
    [TestMethod]
    public async Task DDC2023_Video_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 2000
        };

        var browserContextOptions = new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/",
            RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 }
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync(browserContextOptions);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://www.dotnet-developer-conference.de/");
        await page.GetByTestId("uc-accept-all-button").ClickAsync();
        await page.ClickAsync("text=Programm");
        await page.ClickAsync("id=tag-2-label");
        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        


        Assert.IsTrue(
            page.GetByRole(
                AriaRole.Heading,
                new() { Name = "Testautomatisierung für WebApps mit Playwright" })
            .IsVisibleAsync().Result);
        await context.CloseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region DownloadTest
    [TestMethod]
    public async Task SevenZip_PlaywrightDownloadTest_()
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

    #region DataDriven
    [TestMethod]
    [DataTestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    [DataRow("Edge")]
    [DataRow("Chrome")]
    public async Task DataDriven_DDC2023_SimpleSmokeTest(string BrowserName)
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await GetBrowserAsync(playwright, BrowserName);
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync("https://www.dotnet-developer-conference.de/");
        await page.GetByTestId("uc-accept-all-button").ClickAsync();
        await page.ClickAsync("text=Programm");
        await page.ClickAsync("id=tag-2-label");
        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        Assert.IsTrue(
            page.GetByRole(
                AriaRole.Heading,
                new() { Name = "Testautomatisierung für WebApps mit Playwright" })
            .IsVisibleAsync().Result);
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

    #region DeviceTest
    //[TestMethod]
    public async Task DDC2023_DeviceTest_SmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false
            //,SlowMo = 2000
        };

        // execute test on iPhone 13 landscape
        var device = playwright.Devices["iPhone 13 landscape"];
        device.RecordVideoDir = "videos-iPhone/";
        device.RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync(device);
        var page = await context.NewPageAsync();
        await page.GotoAsync("https://www.dotnet-developer-conference.de/");
        await page.GetByTestId("uc-accept-all-button").ClickAsync();
        await page.ClickAsync("text=Programm");
        await page.ClickAsync("id=tag-2-label");
        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        Assert.IsTrue(
            page.GetByRole(
                AriaRole.Heading,
                new() { Name = "Testautomatisierung für WebApps mit Playwright" })
            .IsVisibleAsync().Result);
        await browser.CloseAsync();
    }
    #endregion

    #region Route Demo
    [TestMethod]
    public async Task DDC_Playwright_RouteBlockTest()
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

        await page.RouteAsync("**/*.jpg", route => route.FulfillAsync(new()
        {
            Status = 404,
            ContentType = "text/plain",
            Body = "Not Found!"
        }));

        await page.GotoAsync("https://www.dotnet-developer-conference.de/programm/#/");
        await page.GetByTestId("uc-accept-all-button").ClickAsync();

        await page.ScreenshotAsync(
            new PageScreenshotOptions
            {
                Path = "SevenZipPage.png",
            });

        await browser.CloseAsync();
    }
    #endregion

    #region Route Advanced
    //await page.RouteAsync("**/*.png", route => route.FulfillAsync(new ()
    /*{
        Status = 404,
        ContentType = "text/plain",
        Body = "Not Found!"
    }));*/

    /*await page.RouteAsync("https://ebnerjobs.de/mediadaten/developer-media-jobs", async route =>
    {
        var response = await route.FetchAsync();
        await route.FulfillAsync(new RouteFulfillOptions
        {
            Response = response,
            Headers = new Dictionary<string, string>(response.Headers)
            {
                ["Content-Disposition"] = "attachment"
                ,//["Content-Type"] = "application/binary"
            }
        });
    });*/
    #endregion
    
}
