using Microsoft.Playwright;

namespace PlayDemo;

//[TestClass]
//[TestCategory("MSTest")]
public class MdDevDays2026PlaywrightSessionTest_Pre
{
    private const string ActOverviewUrl = "https://www.md-devdays.de/act-overview";

    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;

    [TestInitialize]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        _page = await _browser.NewPageAsync();
    }

    [TestCleanup]
    public async Task Teardown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [TestMethod]
    public async Task PlaywrightSession_ExistsAsSessionNotWorkshop_HasAbstractAndSpeakers()
    {
        await _page.GotoAsync(ActOverviewUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Dismiss cookie consent if present
        var cookieButton = _page.GetByText("Speichern").First;
        if (await cookieButton.IsVisibleAsync())
            await cookieButton.ClickAsync();

        // Navigate to the 20.05 tab (Zweiter Konferenztag) where the session is held
        var tab20May = _page.GetByRole(AriaRole.Tab, new() { NameRegex = new System.Text.RegularExpressions.Regex("20\\.05") });
        await tab20May.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Find the card containing "Playwright" — should be exactly one on this tab
        var cards = _page.Locator("mat-card").Filter(new() { HasTextRegex = new System.Text.RegularExpressions.Regex("playwright", System.Text.RegularExpressions.RegexOptions.IgnoreCase) });
        Assert.AreEqual(1, await cards.CountAsync(), "Expected exactly one Playwright card on the 20.05 tab.");

        var sessionCard = cards.First;

        // Verify it is a Session and NOT a Workshop
        await Expect(sessionCard.GetByText("Session")).ToBeVisibleAsync();
        await Expect(sessionCard.GetByText("Workshop")).Not.ToBeVisibleAsync();

        // Verify the card title mentions Playwright
        await Expect(sessionCard.GetByText("Playwright")).ToBeVisibleAsync();

        // Open the detail page via "Mehr Infos"
        var mehrInfosLink = sessionCard.GetByRole(AriaRole.Link, new() { Name = "Mehr Infos" });
        await mehrInfosLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify abstract section (Beschreibung) is present and has content
        await Expect(_page.GetByText("Beschreibung")).ToBeVisibleAsync();

        var abstractPanel = _page.Locator("mat-expansion-panel").Filter(new() { HasText = "Beschreibung" });
        var abstractContent = await abstractPanel.InnerTextAsync();
        Assert.IsTrue(abstractContent.Length > 50, "Abstract (Beschreibung) should contain a meaningful description.");

        // Verify Harald Binkle is listed as a speaker
        await Expect(_page.GetByText("Harald Binkle").First).ToBeVisibleAsync();

        // Verify Nico Orschel is listed as a speaker
        await Expect(_page.GetByText("Nico Orschel").First).ToBeVisibleAsync();
    }

    private static ILocatorAssertions Expect(ILocator locator) =>
        Microsoft.Playwright.Assertions.Expect(locator);
}
