using Microsoft.Playwright;

namespace PlayDemo.Demo;

/// <summary>
/// Live-Demo für die DDC 2026: "Testautomatisierung für WebApps mit Playwright".
/// Der Test läuft immer sichtbar und verlangsamt, damit das Publikum mitlesen kann.
/// </summary>
[TestClass]
[TestCategory("LiveDemo")]
public class DDC2026LiveDemoTests
{
    // Tempo auf der Bühne anpassen, ohne Code zu ändern: DEMO_SLOWMO=500 dotnet test ...
    static int SlowMo => int.TryParse(Environment.GetEnvironmentVariable("DEMO_SLOWMO"), out var ms) ? ms : 1000;

    public TestContext TestContext { get; set; } = null!;

    //[TestMethod] - disabled for now
    public async Task DDC2026_LiveDemo_FindMySession()
    {
        // 1. Browser starten – sichtbar und verlangsamt, damit das Publikum mitlesen kann
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = SlowMo
        });
        await using var context = await browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1280, Height = 800 }
        });
        var page = await context.NewPageAsync();

        // 2. Consent-Banner automatisch wegklicken – egal, ob und wann es erscheint
        await page.AddLocatorHandlerAsync(
            page.GetByRole(AriaRole.Button, new() { Name = "Alles akzeptieren", Exact = true }),
            async button => await button.ClickAsync());

        // 3. Startseite öffnen und wie ein Besucher über das Menü zum DDC-Programm navigieren
        await page.GotoAsync("https://www.developer-world.de/");

        var ddcMenu = page.GetByRole(AriaRole.Button, new() { Name = "Untermenü \"DDC\" anzeigen", Exact = true });
        await ddcMenu.HighlightAsync();
        await ddcMenu.HoverAsync();

        var programLink = page.GetByRole(AriaRole.Link, new() { Name = "Programm", Exact = true });
        await programLink.HighlightAsync();
        await programLink.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync("https://www.developer-world.de/ddc/ddc-programm");

        // 4. Das Programm steckt in einem Sessionize-iFrame
        var program = page.FrameLocator("iframe[src^='https://sessionize.com/']");
        var tuesday = program.GetByRole(AriaRole.Link, new() { Name = "Dienstag", Exact = true });
        await tuesday.HighlightAsync();
        await tuesday.ClickAsync();

        // 5. Session über Rolle + sichtbaren Namen finden – kein CSS, kein XPath
        var session = program.GetByRole(AriaRole.Link, new() { Name = "Testautomatisierung für WebApps mit Playwright", Exact = true });
        await session.ScrollIntoViewIfNeededAsync();
        await session.HighlightAsync();
        await session.ClickAsync();

        // 6. Web-First-Assertions warten automatisch, bis der Dialog da ist
        var dialog = program.Locator("#sz-modal-container");
        var heading = dialog.GetByRole(AriaRole.Heading, new() { Name = "Testautomatisierung für WebApps mit Playwright", Exact = true });
        var speaker = dialog.GetByRole(AriaRole.Link, new() { Name = "Nico Orschel", Exact = true });

        await Assertions.Expect(heading).ToBeVisibleAsync();
        await Assertions.Expect(speaker).ToBeVisibleAsync();

        // 7. Beweisfoto
        await ScrollIntoOuterViewportAsync(page, heading);
        var screenshotPath = Path.Combine(AppContext.BaseDirectory, "screenshot_ddc2026_livedemo.png");
        await page.ScreenshotAsync(new() { Path = screenshotPath });
        TestContext.AddResultFile(screenshotPath);
    }

    /// <summary>Der Dialog liegt im iFrame – die äußere Seite scrollt nicht automatisch mit.</summary>
    static async Task ScrollIntoOuterViewportAsync(IPage page, ILocator locator)
    {
        await locator.ScrollIntoViewIfNeededAsync();
        var bounds = await locator.BoundingBoxAsync();
        Assert.IsNotNull(bounds);
        await page.EvaluateAsync(
            "y => window.scrollBy({ top: y - window.innerHeight / 4, behavior: 'instant' })",
            (double)bounds.Y);
    }
}
