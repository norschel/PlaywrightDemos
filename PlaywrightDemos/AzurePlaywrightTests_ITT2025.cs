using Microsoft.Playwright;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework.Interfaces;
using Microsoft.Playwright.NUnit;
using Azure.Developer.Playwright;
using Azure.Identity;

namespace PlaywrightDemos;

[Parallelizable(ParallelScope.Self)]
[Category("NUnit")]
[TestCategory("NUnit")]
public class AzurePlaywrightTests_ITT2025:CloudBrowserPageTest
{
    private static bool _isHeadless = true;
    private static bool _isEnabledTracing = true;

    [Test]
    public async Task ITT_SimpleSmokeTest_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest2_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest3_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest4_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest5_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest6_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest7_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest8_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest9_NUnit()
    {
        await StartSzenario();
    }

    [Test]
    public async Task ITT_SimpleSmokeTest10_NUnit()
    {
        await StartSzenario();
    }

    private async Task StartSzenario()
    {
        var playwright = this.Playwright;
        
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless
                //,SlowMo = 2000
            });
        var browserContext = await browser.NewContextAsync();
        StartTrace(browserContext);
        var page = await browserContext.NewPageAsync();
        StartTrace(browserContext);
        
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/" + "#" + BrowserName);
        await page.GotoAsync("https://www.ittage.informatik-aktuell.de/programm.html");

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        // use it only for testing or debugging
        //await page.PauseAsync();

        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });
        StopTrace(browserContext, "ITT_SimpleSmokeTest_Tracing");

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }

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

        var tracePath = Path.Combine(
                NUnit.Framework.TestContext.CurrentContext.WorkDirectory,
                $"{testName}_trace.zip",
                $"{testName}_trace.zip"
            );
        NUnit.Framework.TestContext.AddTestAttachment(
            $"{testName}_trace.zip",
            $"{testName}_trace.zip"
        );

    }
    #endregion
}
