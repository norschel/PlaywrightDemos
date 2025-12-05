using Microsoft.Identity.Client;
using Microsoft.Playwright;

namespace PlayDemo;

[TestClass]
public class PlaywrightE2ETests_IT_Tage_2025
{
    #region Globals
    static bool _isHeadless = false;
    #endregion

    #region SimpleSmokeTest
    [TestMethod]
    public async Task ITT_SimpleSmokeTest()
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

        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/");
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

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
    public async Task ITT_DataDrivenSmokeTest(string BrowserName)
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await GetBrowserAsync(playwright, BrowserName);
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/" + "#" + BrowserName);
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

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
    public async Task ITT_DeviceTest()
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

        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/");
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region VideoTest
    [TestMethod]
    public async Task ITT_VideoSimpleTest()
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

        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/");
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region Network Requests

    [TestMethod]
    public async Task ITT_NetworkRequest_Simple_Test()
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

        await page.RouteAsync("**/*.jpg", route => route.FulfillAsync(new()
        {
            Status = 404,
            ContentType = "text/plain",
            Body = "Not Found!"
        }));

        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/");
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_before.png" });

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_before.png" });
        //await page.PauseAsync();

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }

    [TestMethod]
    public async Task ITT_NetworkRequest_FullTest()
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

        //https://www.ittage.informatik-aktuell.de/fileadmin…ocessed_/e/3/csm_300-Harald-Binkle_4245c411b0.jpg
        await page.RouteAsync("**/*Harald-Binkle*.jpg", async route =>
            {
                var response = await route.FetchAsync();

                var body = await response.BodyAsync();
                // read local image and replace the response body

                var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
                var filePath = Path.Combine(assemblyDir, "testdaten", "santa_harry.jpg");
                body = await File.ReadAllBytesAsync(filePath);

                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Response = response,
                    BodyBytes = body,
                    Headers = new Dictionary<string, string>(response.Headers)
                    {
                        ["Content-Type"] = "application/image-jpeg",
                    }
                });
            });

        await page.RouteAsync("**/*Nico-Orschel*.jpg", async route =>
        {
            var response = await route.FetchAsync();

            var body = await response.BodyAsync();
            // read local image and replace the response body

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "santa_nico.jpg");
            body = await File.ReadAllBytesAsync(filePath);

            await route.FulfillAsync(new RouteFulfillOptions
            {
                Response = response,
                BodyBytes = body,
                Headers = new Dictionary<string, string>(response.Headers)
                {
                    ["Content-Type"] = "application/image-jpeg",
                }
            });
        });


        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/");
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_before.png" });

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_before.png" });
        //await page.PauseAsync();

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion
}