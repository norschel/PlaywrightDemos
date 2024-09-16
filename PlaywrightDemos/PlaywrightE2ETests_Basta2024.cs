using System.Diagnostics.CodeAnalysis;
using Microsoft.Playwright;

namespace demo123;

[TestClass]
public class Basta2024_Demos
{
    #region Globals
    static bool _isHeadless = false;
    static bool _isEnabledTracing = true;
    #endregion

    //playwright.Selectors.SetTestIdAttribute("id");

    //var tag2Label = page.GetByTestId("tag-2-label");
    //await tag2Label.ClickAsync();

    #region SimpleSmokeTest
    [TestMethod]
    public async Task Basta2024_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
            SlowMo = 2000
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/mainz/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta2024_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta2024_2.png" });
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day2]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day2]").First.HighlightAsync();
        await page.Locator("[href*=Day2]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta2024_3.png" });

        Assert.IsTrue(page.TitleAsync().Result.Contains("Playwright"));
        //await context.CloseAsync();
        //await browser.CloseAsync();
        Basta2024_Demos.StopTrace(context, "Basta2024_SimpleSmokeTest");
    }
    #endregion

    #region Video
    [TestMethod]
    public async Task Basta2024_Video_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
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
        await page.GotoAsync("https://basta.net/mainz/");
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day2]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day2]").First.HighlightAsync();
        await page.Locator("[href*=Day2]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta2024.png" });

        Assert.IsTrue(page.TitleAsync().Result.Contains("Playwright"));
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
            await page.Locator("text=ct_2024").ClickAsync();
        });

        await task.Result.SaveAsAsync("mediadaten_ct_2024.pdf");

        Assert.IsTrue(File.Exists("mediadaten_ct_2024.pdf"));

        await browser.CloseAsync();

    }
    #endregion

    #region DataDriven
    [TestMethod]
    [DataTestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    public async Task DataDriven_Basta2023_SimpleSmokeTest(string BrowserName)
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await GetBrowserAsync(playwright, BrowserName);
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/mainz/");
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day2]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day2]").First.HighlightAsync();
        await page.Locator("[href*=Day2]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta2024.png" });

        Assert.IsTrue(page.TitleAsync().Result.Contains("Playwright"));
        await context.CloseAsync();
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
            default:
                throw new ArgumentException("Browser not supported");
        }
    }
    #endregion

    #region DeviceTest
    [TestMethod]
    public async Task Basta2023_DeviceTest_SmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless
            //,SlowMo = 2000
        };

        // execute test on iPhone 13 landscape
        var device = playwright.Devices["iPhone 13 landscape"];
        device.RecordVideoDir = "videos-iPhone/";
        device.RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync(device);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/mainz/");
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day2]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day2]").First.HighlightAsync();
        await page.Locator("[href*=Day2]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta2023.png" });

        Assert.IsTrue(page.TitleAsync().Result.Contains("Playwright"));
        await context.CloseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region Route Demo
    [TestMethod]
    public async Task Basta2023_Playwright_RouteBlockTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless,
                SlowMo = 5000
            });

        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await page.RouteAsync("**/*.{png,jpg,jpeg,svg}", route => route.FulfillAsync(new()
        {
            Status = 404,
            ContentType = "text/plain",
            Body = "Not Found!"
        }));

        await page.GotoAsync("https://www.basta.net/mainz/");

        await page.ScreenshotAsync(
            new PageScreenshotOptions
            {
                Path = "BastaPage.png",
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

#region Helper

public async void StartTrace(IBrowserContext context)
{
    if (!_isEnabledTracing)
    {
        return;
    }

    await context.Tracing.StartAsync(new TracingStartOptions
    {
        Screenshots = true,
        Snapshots = true,
        Sources = true
    });
}

    public static void StopTrace(IBrowserContext context, string testName)
    {
        if (!_isEnabledTracing)
        {
            return;
        }

        var traceOptions = new TracingStopOptions
        {
            Path = testName + "_trace.zip"
            //Path = "trace.zip"
        };
        context.Tracing.StopAsync(traceOptions).Wait();
    }
    #endregion

}