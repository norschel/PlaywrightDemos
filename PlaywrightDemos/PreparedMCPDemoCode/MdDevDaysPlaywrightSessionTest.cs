using Microsoft.Playwright;

namespace PlayDemo;

//[TestClass]
#pragma warning disable MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
public class MdDevDaysPlaywrightSessionTest
#pragma warning restore MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
{
    private const string SessionsOverviewUrl = "https://www.md-devdays.de/act-overview";

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
    public async Task PlaywrightSession_IsSessionNotWorkshop_HasAbstractAndBothSpeakers()
    {
        // Open sessions overview
        await _page.GotoAsync(SessionsOverviewUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Dismiss cookie consent banner if it appears
        var cookieSaveButton = _page.GetByRole(AriaRole.Button, new() { Name = "Speichern" });
        if (await cookieSaveButton.IsVisibleAsync())
            await cookieSaveButton.ClickAsync();

        // Navigate to the second conference day tab (20.05) where the Playwright session is scheduled
        var tab20May = _page.GetByRole(AriaRole.Tab, new()
        {
            NameRegex = new System.Text.RegularExpressions.Regex(@"20\.05", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        });
        await Expect(tab20May).ToBeVisibleAsync(new() { Timeout = 10_000 });
        await tab20May.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Find the card that contains "Playwright" in its text
        var playwrightCards = _page.Locator("mat-card").Filter(new()
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex("playwright", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        });

        var cardCount = await playwrightCards.CountAsync();
        Assert.IsTrue(cardCount > 0, "Es wurde keine Session gefunden, die 'Playwright' enthält.");

        // Select the first card that is a Session (not a Workshop)
        ILocator? sessionCard = null;
        for (int i = 0; i < cardCount; i++)
        {
            var candidate = playwrightCards.Nth(i);
            var cardText = await candidate.InnerTextAsync();

            // Must contain "Session" but must NOT be a Workshop
            if (cardText.Contains("Session", StringComparison.OrdinalIgnoreCase) &&
                !cardText.Contains("Workshop", StringComparison.OrdinalIgnoreCase))
            {
                sessionCard = candidate;
                break;
            }
        }

        Assert.IsNotNull(sessionCard, "Es wurde keine Playwright-Session (kein Workshop) auf dem Tab '20.05' gefunden.");

        // Verify the card explicitly shows "Session" (not "Workshop")
        var sessionCardText = await sessionCard.InnerTextAsync();
        Assert.IsTrue(sessionCardText.Contains("Session", StringComparison.OrdinalIgnoreCase),
            "Die gefundene Karte enthält nicht den Eintrag 'Session'.");
        Assert.IsFalse(sessionCardText.Contains("Workshop", StringComparison.OrdinalIgnoreCase),
            "Die gefundene Karte ist ein Workshop, keine Session.");

        // Open the detail page via "Mehr Infos"
        var mehrInfosLink = sessionCard.GetByRole(AriaRole.Link, new() { Name = "Mehr Infos" });
        await Expect(mehrInfosLink).ToBeVisibleAsync(new() { Timeout = 5_000 });
        await mehrInfosLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // --- Abstract check ---
        // The "Beschreibung" expansion panel should be present and contain meaningful text
        var abstractPanel = _page.Locator("mat-expansion-panel").Filter(new() { HasText = "Beschreibung" });
        await Expect(abstractPanel).ToBeVisibleAsync(new() { Timeout = 10_000 });

        // Expand the panel if it is not already expanded
        var panelHeader = abstractPanel.Locator("mat-expansion-panel-header");
        var isExpanded = await panelHeader.GetAttributeAsync("aria-expanded");
        if (isExpanded != "true")
            await panelHeader.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var abstractText = await abstractPanel.InnerTextAsync();

        // Remove the panel title from the text before measuring length
        var abstractBody = abstractText.Replace("Beschreibung", string.Empty).Trim();
        Assert.IsTrue(abstractBody.Length > 50,
            $"Das Abstract (Beschreibung) ist zu kurz oder leer. Gefunden: '{abstractBody}'");

        // --- Speaker check ---
        // The "Speaker-Infos" expansion panel must list both Harald and Nico
        var speakerPanel = _page.Locator("mat-expansion-panel").Filter(new() { HasText = "Speaker-Infos" });
        await Expect(speakerPanel).ToBeVisibleAsync(new() { Timeout = 10_000 });

        var speakerPanelHeader = speakerPanel.Locator("mat-expansion-panel-header");
        var isSpeakerExpanded = await speakerPanelHeader.GetAttributeAsync("aria-expanded");
        if (isSpeakerExpanded != "true")
            await speakerPanelHeader.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var speakerText = await speakerPanel.InnerTextAsync();

        Assert.IsTrue(speakerText.Contains("Harald", StringComparison.OrdinalIgnoreCase),
            "'Harald' wurde in den Speaker-Informationen nicht gefunden.");
        Assert.IsTrue(speakerText.Contains("Nico", StringComparison.OrdinalIgnoreCase),
            "'Nico' wurde in den Speaker-Informationen nicht gefunden.");
    }

    private static ILocatorAssertions Expect(ILocator locator) =>
        Microsoft.Playwright.Assertions.Expect(locator);
}
