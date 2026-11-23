using System.Collections.Concurrent;
using Microsoft.Playwright;

namespace PlayDemo;

[TestClass]
[TestCategory("MSTest")]
[TestCategory("CICD")]
public class PlaywrightE2ETests_DDC2026
{
    #region Globals
    const string ProgramUrl = "https://www.developer-world.de/ddc/ddc-programm";
    const string ProgramFrameSelector = "iframe[src^='https://sessionize.com/']";
    const string SessionTitle = "Testautomatisierung für WebApps mit Playwright";
    const string SpeakerName = "Nico Orschel";

    static bool _isHeadless = false;
    static bool _localDemoMode = true;
    static bool IsLocalDemoMode => !IsRunningOnBuildServer() && _localDemoMode;
    static bool IsHeadless => IsRunningOnBuildServer() || _isHeadless;

    static readonly int _slomo = 2000;
    static int Slomo => IsRunningOnBuildServer() ? 0 : _slomo;

    static bool IsRunningOnBuildServer() =>
        Environment.GetEnvironmentVariable("CI") == "true" ||
        Environment.GetEnvironmentVariable("TF_BUILD") == "True" ||
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")) ||
        IsRunningInContainer();

    static bool IsRunningInContainer() =>
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
        File.Exists("/.dockerenv");

    internal static async Task OpenPlaywrightSessionAsync(IPage page)
    {
        // The optional consent banner can appear while the embedded program is loading.
        await page.AddLocatorHandlerAsync(
            page.GetByRole(AriaRole.Button, new() { Name = "Alles akzeptieren", Exact = true }),
            async button => await button.ClickAsync());
        await page.AddLocatorHandlerAsync(
            page.Locator("getsitecontrol-widget button.close"),
            async button => await button.ClickAsync());

        await page.GotoAsync(ProgramUrl);

        var program = page.FrameLocator(ProgramFrameSelector);
        await program.GetByRole(AriaRole.Link, new() { Name = "Dienstag", Exact = true }).ClickAsync();

        var sessionLink = program.GetByRole(AriaRole.Link, new() { Name = SessionTitle, Exact = true });
        await sessionLink.ScrollIntoViewIfNeededAsync();
        if (IsLocalDemoMode)
        {
            await sessionLink.HighlightAsync();
        }
        await sessionLink.ClickAsync();

        await ScrollSessionIntoViewAsync(page);
    }

    static async Task ScrollSessionIntoViewAsync(IPage page)
    {
        var heading = page.FrameLocator(ProgramFrameSelector).Locator("#sz-modal-container")
            .GetByRole(AriaRole.Heading, new() { Name = SessionTitle, Exact = true });
        await heading.ScrollIntoViewIfNeededAsync();
        var bounds = await heading.BoundingBoxAsync();
        Assert.IsNotNull(bounds);

        // The fixed iframe modal does not scroll the outer page into view.
        await page.EvaluateAsync(
            "y => window.scrollBy({ top: y - window.innerHeight / 4, behavior: 'instant' })",
            (double)bounds.Y);
    }

    internal static async Task AssertSessionModalAsync(IPage page)
    {
        var modal = page.FrameLocator(ProgramFrameSelector).Locator("#sz-modal-container");
        await Assertions.Expect(modal.GetByRole(AriaRole.Heading, new() { Name = SessionTitle, Exact = true }))
            .ToBeVisibleAsync();
        await Assertions.Expect(modal.GetByRole(AriaRole.Heading, new() { Name = SessionTitle, Exact = true }))
            .ToBeInViewportAsync();
        await Assertions.Expect(modal.GetByRole(AriaRole.Link, new() { Name = SpeakerName, Exact = true }))
            .ToBeVisibleAsync();
    }
    #endregion

    #region SimpleSmokeTest
    [TestMethod]
    public async Task DDC2026_SimpleSmokeTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");

        await using var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await OpenPlaywrightSessionAsync(page);
        await AssertSessionModalAsync(page);
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_ddc2026.png" });

        if (IsLocalDemoMode)
        {
            await page.PauseAsync();
        }
    }
    #endregion

    #region DataDrivenSmokeTest
    [TestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    [DataRow("Edge")]
    [DataRow("Chrome")]
    public async Task DDC2026_DataDrivenSmokeTest(string BrowserName)
    {
        if (ShouldSkipInContainer(BrowserName))
        {
            return;
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, BrowserName);
        await using var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await OpenPlaywrightSessionAsync(page);
        await AssertSessionModalAsync(page);
    }

    private static bool ShouldSkipInContainer(string BrowserName)
    {
        if (IsRunningInContainer() && (BrowserName == "Edge" || BrowserName == "Chrome"))
        {
            Assert.Inconclusive($"Browser channel '{BrowserName}' is not supported in Docker containers. Skipping test.");
            return true;
        }

        return false;
    }

    private static async Task<IBrowser> GetBrowserAsync(IPlaywright playwright, string BrowserName, float deviceScaleFactor = 1)
    {
        var browserOptions = new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = Slomo
        };

        if (OperatingSystem.IsMacOS() && !IsHeadless && BrowserName is ("Chromium" or "Edge" or "Chrome"))
        {
            // Keep Retina iframe coordinates consistent with the emulated viewport.
            browserOptions.Args = [$"--force-device-scale-factor={deviceScaleFactor.ToString(System.Globalization.CultureInfo.InvariantCulture)}"];
        }

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
    [TestMethod]
    public async Task DDC2026_DeviceTest()
    {
        using var playwright = await Playwright.CreateAsync();

        // execute test on iPhone 13 landscape
        var device = playwright.Devices["iPhone 13 landscape"];
        await using var browser = await GetBrowserAsync(playwright, "Chromium", device.DeviceScaleFactor ?? 1);
        await using var browserContext = await browser.NewContextAsync(device);
        var page = await browserContext.NewPageAsync();

        await OpenPlaywrightSessionAsync(page);
        await AssertSessionModalAsync(page);
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_ddc2026_iphone.png" });
    }
    #endregion

    #region VideoTest
    [TestMethod]
    public async Task DDC2026_VideoSimpleTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");

        await using var browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/",
            RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 }
        });
        var page = await browserContext.NewPageAsync();

        await OpenPlaywrightSessionAsync(page);
        await AssertSessionModalAsync(page);
    }
    #endregion

    #region TracingTest
    [TestMethod]
    public async Task DDC2026_TracingTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");

        await using var browserContext = await browser.NewContextAsync();
        await browserContext.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        var page = await browserContext.NewPageAsync();
        try
        {
            await OpenPlaywrightSessionAsync(page);
            await AssertSessionModalAsync(page);
        }
        finally
        {
            await browserContext.Tracing.StopAsync(new TracingStopOptions { Path = "trace_ddc2026.zip" });
        }

        Assert.IsTrue(File.Exists("trace_ddc2026.zip"));
    }
    #endregion

    #region Route Network Mocking
    [TestMethod]
    public async Task DDC2026_NetworkRequest_BlockImagesTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");

        await using var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        var blockedImages = 0;
        await page.RouteAsync("**/*.{png,jpg,jpeg,webp}", async route =>
        {
            blockedImages++;
            await route.AbortAsync();
        });

        await OpenPlaywrightSessionAsync(page);
        await AssertSessionModalAsync(page);

        Assert.IsGreaterThan(0, blockedImages);
    }
    #endregion

    #region Xebia Logo
    [TestMethod]
    public async Task DDC2026_NetworkRequest_XebiaLogoForAllImagesTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");
        await using var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var xebiaLogo = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "testdaten", "xebia_logo.svg"));
        var replacedImageUrls = new ConcurrentDictionary<string, byte>();
        // Match by resource type because many image URLs (ads, CDNs) have no file extension.
        await page.RouteAsync("**/*", async route =>
        {
            if (route.Request.ResourceType != "image")
            {
                await route.FallbackAsync();
                return;
            }

            replacedImageUrls.TryAdd(route.Request.Url, 0);
            await route.FulfillAsync(new()
            {
                ContentType = "image/svg+xml",
                BodyBytes = xebiaLogo
            });
        });

        await OpenPlaywrightSessionAsync(page);
        await AssertSessionModalAsync(page);
        await AssertAllImagesShowXebiaLogoAsync(page, replacedImageUrls);
        await page.ScreenshotAsync(new() { Path = "screenshot_ddc2026_xebia.png", FullPage = true });

        if (IsLocalDemoMode)
        {
            await page.PauseAsync();
        }
    }
    #endregion

    #region Speaker Photos
    [TestMethod]
    public async Task DDC2026_SpeakerPhotosTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");
        await using var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await OpenPlaywrightSessionAsync(page);
        var speakers = await GetSessionSpeakerPhotosAsync(page);
        await AddSessionSpeakerPhotosAsync(page, speakers);
        await AssertSessionSpeakerPhotosAsync(page, speakers);
        await page.ScreenshotAsync(new() { Path = "screenshot_ddc2026_speakers.png" });

        if (IsLocalDemoMode)
        {
            await page.PauseAsync();
        }
    }
    #endregion

    #region Let It Snow
    [TestMethod]
    public async Task DDC2026_NetworkRequest_Full_LetItSnowTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await GetBrowserAsync(playwright, "Chromium");
        await using var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await OpenPlaywrightSessionAsync(page);
        var speakers = await GetSessionSpeakerPhotosAsync(page);
        var nico = Assert.ContainsSingle(speakers.Where(speaker => speaker.Name == SpeakerName));
        var santaImage = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "testdaten", "santa_nico.jpg"));
        var replacedPhotos = 0;
        await page.RouteAsync(nico.ImageUrl, async route =>
        {
            await route.FulfillAsync(new()
            {
                ContentType = "image/jpeg",
                BodyBytes = santaImage
            });
            Interlocked.Increment(ref replacedPhotos);
        });

        await AddSessionSpeakerPhotosAsync(page, speakers);
        await AssertSessionSpeakerPhotosAsync(page, speakers);
        Assert.IsGreaterThan(0, replacedPhotos, "The speaker portrait should be replaced by the Santa photo.");

        var snowScript = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "testdaten", "let-it-snow.js"));
        await page.EvaluateAsync(snowScript, new { playMusic = !IsHeadless, showSnowmen = true });

        var snowmen = page.Locator("#snowmen-layer");
        await Assertions.Expect(snowmen).ToHaveCSSAsync("pointer-events", "none");
        Assert.IsTrue(await snowmen.EvaluateAsync<bool>("""
            async layer => {
                const walkers = ['left', 'right'].map(direction =>
                    layer.querySelector(`.snowman[data-direction="${direction}"]`));
                if (walkers.some(walker => !walker)) throw new Error('Missing initial snowmen in both directions');
                const before = walkers.map(walker => walker.getBoundingClientRect().x);
                await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
                return walkers.every((walker, index) => {
                    const bounds = walker.getBoundingClientRect();
                    const moving = walker.dataset.direction === 'left'
                        ? bounds.x < before[index]
                        : bounds.x > before[index];
                    return moving && bounds.bottom <= innerHeight && bounds.bottom >= innerHeight - 8;
                });
            }
            """), "Snowmen should walk in both directions along the bottom of the viewport.");

        var snow = page.Locator("#snow-canvas");
        await Assertions.Expect(snow).ToBeVisibleAsync();
        await Assertions.Expect(snow).ToHaveCSSAsync("pointer-events", "none");
        Assert.IsTrue(await snow.EvaluateAsync<bool>("""
            async canvas => {
                const context = canvas.getContext('2d');
                const before = context.getImageData(0, 0, canvas.width, canvas.height).data;
                await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
                const after = context.getImageData(0, 0, canvas.width, canvas.height).data;
                return after.some((value, index) => value !== before[index]);
            }
            """), "The snow animation should change the rendered pixels.");
        await page.ScreenshotAsync(new() { Path = "screenshot_ddc2026_christmas.png" });

        if (IsLocalDemoMode)
        {
            await page.PauseAsync();
        }

        var modal = page.FrameLocator(ProgramFrameSelector).Locator("#sz-modal-container");
        await modal.GetByRole(AriaRole.Button, new() { Name = "✕", Exact = true }).ClickAsync();
        await Assertions.Expect(modal).ToBeHiddenAsync();
    }
    #endregion

    #region Speaker Photo Helpers
    private sealed class SpeakerPhoto
    {
        public required string Name { get; set; }
        public required string ImageUrl { get; set; }
    }

    static async Task<SpeakerPhoto[]> GetSessionSpeakerPhotosAsync(IPage page)
    {
        var speakers = await page.FrameLocator(ProgramFrameSelector)
            .Locator("#sz-modal-container .sz-session__speakers a")
            .EvaluateAllAsync<SpeakerPhoto[]>("""
                async links => Promise.all(links.map(async link => {
                    const name = link.textContent.trim();
                    const match = link.getAttribute('onclick')?.match(/showModal\('([^']+)',\s*'speaker',\s*'([^']+)'\)/);
                    if (!match) throw new Error(`Missing Sessionize speaker endpoint for ${name}`);

                    const response = await fetch(`https://sessionize.com/api/v2/${match[1]}/speaker?id=${match[2]}`, {
                        method: 'POST'
                    });
                    if (!response.ok) throw new Error(`Speaker request for ${name} failed: HTTP ${response.status}`);

                    const document = new DOMParser().parseFromString(await response.text(), 'text/html');
                    const source = document.querySelector('.sz-speaker__photo img')?.getAttribute('src');
                    if (!source) throw new Error(`Missing speaker portrait for ${name}`);
                    return { Name: name, ImageUrl: new URL(source, response.url).href };
                }))
                """);
        Assert.IsNotEmpty(speakers, "The session should contain speakers with portraits.");
        return speakers;
    }

    static async Task AddSessionSpeakerPhotosAsync(IPage page, SpeakerPhoto[] speakers)
    {
        var modal = page.FrameLocator(ProgramFrameSelector).Locator("#sz-modal-container");
        await modal.EvaluateAsync("""
            async (modal, speakers) => {
                const list = modal.querySelector('.sz-session__speakers');
                if (!list) throw new Error('Missing speaker list in the session dialog');
                Object.assign(list.style, {
                    display: 'flex', flexWrap: 'wrap', gap: '16px', padding: '0', margin: '0 0 16px'
                });

                for (const speaker of speakers) {
                    const link = Array.from(list.querySelectorAll('a'))
                        .find(link => link.textContent.trim() === speaker.Name);
                    if (!link) throw new Error(`Missing session speaker link for ${speaker.Name}`);
                    const item = link.closest('li');
                    if (!item) throw new Error(`Missing speaker list item for ${speaker.Name}`);
                    Object.assign(item.style, { display: 'flex', alignItems: 'center', gap: '12px' });

                    const image = new Image(96, 96);
                    image.alt = speaker.Name;
                    image.dataset.testid = 'ddc-speaker-photo';
                    Object.assign(image.style, {
                        width: '96px', height: '96px', objectFit: 'cover', borderRadius: '50%', flexShrink: '0'
                    });
                    item.prepend(image);
                    image.src = speaker.ImageUrl;
                    await image.decode();
                }
            }
            """, speakers);
        await ScrollSessionIntoViewAsync(page);
    }

    static async Task AssertSessionSpeakerPhotosAsync(IPage page, SpeakerPhoto[] speakers)
    {
        await AssertSessionModalAsync(page);
        var modal = page.FrameLocator(ProgramFrameSelector).Locator("#sz-modal-container");
        await Assertions.Expect(modal.GetByTestId("ddc-speaker-photo")).ToHaveCountAsync(speakers.Length);
        foreach (var speaker in speakers)
        {
            var image = modal.GetByRole(AriaRole.Img, new() { Name = speaker.Name, Exact = true });
            await Assertions.Expect(image).ToBeInViewportAsync();
            Assert.IsGreaterThan(0, await image.EvaluateAsync<int>("image => image.naturalWidth"),
                $"The portrait of {speaker.Name} should load in the session dialog.");
        }
    }
    #endregion

    #region Xebia Logo Helpers
    private sealed class PageImage
    {
        public required string Url { get; set; }
        public required bool IsLoaded { get; set; }
    }

    static async Task AssertAllImagesShowXebiaLogoAsync(IPage page, ConcurrentDictionary<string, byte> replacedImageUrls)
    {
        var images = new List<PageImage>();
        foreach (var frame in page.Frames)
        {
            try
            {
                images.AddRange(await frame.EvaluateAsync<PageImage[]>("""
                    async () => {
                        const images = Array.from(document.images);
                        // Load lazy images outside the viewport, too.
                        images.forEach(image => image.loading = 'eager');
                        await Promise.all(images.map(image => image.decode().catch(() => {})));
                        return images
                            .filter(image => /^https?:/.test(image.currentSrc))
                            .map(image => {
                                const url = new URL(image.currentSrc);
                                url.hash = '';
                                return { Url: url.href, IsLoaded: image.naturalWidth > 0 };
                            });
                    }
                    """));
            }
            catch (PlaywrightException) when (frame.IsDetached)
            {
                // Ad frames can be replaced while the page is inspected.
            }
        }

        Assert.IsNotEmpty(images, "The page should contain images.");
        var otherImages = images
            .Where(image => !image.IsLoaded || !replacedImageUrls.ContainsKey(image.Url))
            .Select(image => image.Url)
            .ToArray();
        Assert.IsEmpty(otherImages, $"Every image should show the Xebia logo, but these did not: {string.Join(", ", otherImages)}");
    }
    #endregion
}
