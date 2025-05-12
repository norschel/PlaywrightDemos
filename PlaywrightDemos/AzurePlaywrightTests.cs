using Azure.Developer.MicrosoftPlaywrightTesting.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace PlaywrightTests;

 [SetUpFixture]
public class AzurePlaywrightTests:PlaywrightServiceNUnit
{
   
    private static bool _isHeadless = true;

    [Test]
    public async Task MDD_SimpleSmokeTest_NUnit()
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
        await page.GotoAsync("https://md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Session-Übersicht").First.ClickAsync();
        //await page.GetByRole(AriaRole.Tab, new() { Name = "14.05." }).ClickAsync();
        await page.Locator("id=mat-tab-label-0-1").ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
        var sessionLink = page.Locator(".act-card-content-container").
            Filter(new() { HasText = "Testautomatisierung für WebApps mit Playwright" }).
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
}
