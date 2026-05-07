using Microsoft.Playwright;

namespace PlayDemo;

[TestClass]
[TestCategory("MSTest")]
[TestCategory("CICD")]
#pragma warning disable MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
public class PlaywrightE2ETests_MDD2026
#pragma warning restore MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
{
    #region Globals
    static bool _isHeadless = false;
    static bool localDemoMode = false;
    static bool IsHeadless => IsRunningOnBuildServer() || _isHeadless;

    static readonly int _slomo = 2000;

    static bool IsRunningOnBuildServer() =>
        Environment.GetEnvironmentVariable("CI") == "true" ||
        Environment.GetEnvironmentVariable("TF_BUILD") == "True" ||
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")) ||
        IsRunningInContainer();

    static bool IsRunningInContainer() =>
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
        File.Exists("/.dockerenv");
    #endregion

    #region SimpleSmokeTest
    [TestMethod]
    public async Task MDD_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = IsHeadless,
                SlowMo = _slomo,
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
            Filter(new() { HasText = "Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");

        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        if (localDemoMode)
        {
            await page.PauseAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
            await page.GetByText("Der Workshop \"Testautomatisierung mit Playwright\" bietet eine umfassende Einführung").IsVisibleAsync());

        if (localDemoMode)
        {
            await page.PauseAsync();
        }
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
        #region containerCheck
        bool containerCheck = ContainerCheck(BrowserName);
        if (!containerCheck)
        {
            return;
        }
        #endregion

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
            Filter(new() { HasText = "Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
                   await page.GetByText("Der Workshop \"Testautomatisierung mit Playwright\" bietet eine umfassende Einführung").IsVisibleAsync());

        if (localDemoMode)
        {
            await page.PauseAsync();
        }
        await browser.CloseAsync();
    }

    private static bool ContainerCheck(string BrowserName)
    {
        if (IsRunningInContainer() 
        && (BrowserName == "Edge" 
        || BrowserName == "Chrome"))
        {
            Assert.Inconclusive($"Browser channel '{BrowserName}' is not supported in Docker containers. Skipping test.");
            return false;
        }

        return true;
    }

    private static async Task<IBrowser> GetBrowserAsync(IPlaywright playwright, string BrowserName)
    {
        var browserOptions = new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = _slomo
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
                SlowMo = _slomo,
            });
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();
        await page.GotoAsync("https://mediadaten.heise.de/en/home/rate-cards/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "heise_ratecards.png" });

        var task = page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.Locator("[href*=ct_RateCard2026]").ClickAsync();
        }
        );

        await task.Result.SaveAsAsync("mediadaten_ct_2026.pdf");

        Assert.IsTrue(File.Exists("mediadaten_ct_2026.pdf"));

        await browser.CloseAsync();
    }

    [TestMethod]
    public async Task SevenZip_PlaywrightDownloadTest_()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = IsHeadless,
                SlowMo = _slomo
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
                Headless = IsHeadless,
                SlowMo = _slomo
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
            Filter(new() { HasText = "Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        if (localDemoMode)
        {
            await page.PauseAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
                   await page.GetByText("Der Workshop \"Testautomatisierung mit Playwright\" bietet eine umfassende Einführung").IsVisibleAsync());

        if (localDemoMode)
        {
            await page.PauseAsync();
        }
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
                Headless = IsHeadless,
                SlowMo = _slomo
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
            Filter(new() { HasText = "Bootcamp - Testautomatisierung mit Playwright" }).
            GetByText("Mehr Infos");
        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

        Assert.IsTrue(
                   await page.GetByText("Der Workshop \"Testautomatisierung mit Playwright\" bietet eine umfassende Einführung").IsVisibleAsync());

        if (localDemoMode)
        {
            await page.PauseAsync();
        }
        await browser.CloseAsync();
    }
    #endregion

    #region Route Advanced

    [TestMethod]
    public async Task MDD2026_NetworkRequest_FullTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = _slomo
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await page.RouteAsync("**/em/Media2/File/32fc283c-7fc4-4809-a066-7b1439bb081d", async route =>
        {
            var response = await route.FetchAsync();
            var body = await response.BodyAsync();

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "zwerg_harry.jpg");
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

        await page.RouteAsync("**/em/Media2/File/5ef7df86-c4ff-4345-8fd5-007d6dac7c70", async route =>
        {
            var response = await route.FetchAsync();
            var body = await response.BodyAsync();

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "zwerg_nico.jpg");
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

        await page.GotoAsync("https://www.md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Sessions").First.ClickAsync();
        await page.Locator("id=mat-tab-label-0-0").ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();

        var sessionLink = page.Locator(".act-card-content-container")
            .Filter(new() { HasText = "Bootcamp - Testautomatisierung mit Playwright" })
            .GetByText("Mehr Infos");

        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_route.png" });

        if (localDemoMode)
        {
            await page.PauseAsync();
        }
        await browser.CloseAsync();
    }

    [TestMethod]
    public async Task MDD2026_NetworkRequest_FullTest_TanzInDenMai()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = _slomo
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var browserContext = await browser.NewContextAsync();
        var page = await browserContext.NewPageAsync();

        await page.RouteAsync("**/em/Media2/File/32fc283c-7fc4-4809-a066-7b1439bb081d", async route =>
        {
            var response = await route.FetchAsync();
            var body = await response.BodyAsync();

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "zwerg_harry.jpg");
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

        await page.RouteAsync("**/em/Media2/File/5ef7df86-c4ff-4345-8fd5-007d6dac7c70", async route =>
        {
            var response = await route.FetchAsync();
            var body = await response.BodyAsync();

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "zwerg_nico.jpg");
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

        await page.GotoAsync("https://www.md-devdays.de/home");
        await page.Locator("text=Speichern").First.ClickAsync();
        await page.Locator("text=Sessions").First.ClickAsync();
        await page.Locator("id=mat-tab-label-0-0").ClickAsync();
        await page.Locator("text=Playwright").HighlightAsync();
        await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();

        var sessionLink = page.Locator(".act-card-content-container")
            .Filter(new() { HasText = "Bootcamp - Testautomatisierung mit Playwright" })
            .GetByText("Mehr Infos");

        await sessionLink.ScrollIntoViewIfNeededAsync();
        await sessionLink.HighlightAsync();
        await sessionLink.ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_mai.png" });

        await page.EvaluateAsync(@"
            () => {
                if (document.getElementById('mai-canvas')) return;

                const canvas = document.createElement('canvas');
                canvas.id = 'mai-canvas';
                Object.assign(canvas.style, {
                    position: 'fixed',
                    left: '0',
                    top: '0',
                    width: '100%',
                    height: '100%',
                    pointerEvents: 'none',
                    zIndex: '9999',
                });
                document.body.appendChild(canvas);

                const ctx = canvas.getContext('2d');

                // Musik: ""Der Mai ist gekommen"" (3/4 Walzertakt, D-Dur)
                if (!window.__maiLoop) {
                    const AudioCtx = window.AudioContext || window.webkitAudioContext;
                    if (AudioCtx) {
                        const ctxAudio = new AudioCtx();
                        const tempo = 126;
                        const beat = 60 / tempo;
                        // D-Dur: A4=440 H4=494 Fis4=370 E4=330 G4=392 D5=587
                        const melody = [
                            // ""Der Mai ist ge-kom-men,""
                            [440,0.5],[440,0.5],[440,0.5],
                            [370,0.5],[440,0.5],[494,0.5],
                            [440,1.5],
                            // ""die Bäu-me schla-gen aus,""
                            [440,0.5],[440,0.5],[370,0.5],
                            [330,1.5],
                            // ""da-heim bleibt wer Lust hat,""
                            [370,0.5],[370,0.5],[370,0.5],
                            [330,0.5],[370,0.5],[392,0.5],
                            [440,1.5],
                            // ""wen Gott nicht rei-sen lässt.""
                            [440,1.0],[440,0.5],
                            [440,1.5],
                            // ""Wohl ü-ber die Ber-ge,""
                            [494,0.5],[494,0.5],[494,0.5],
                            [440,0.5],[494,0.5],[587,0.5],
                            [494,1.5],
                            // ""wohl durch das tie-fe Tal,""
                            [440,0.5],[440,0.5],[494,0.5],
                            [440,1.5],
                            // ""der Wald ist wie ein Haus,""
                            [370,0.5],[370,0.5],[370,0.5],
                            [330,0.5],[370,0.5],[392,0.5],
                            [440,1.5],
                            // ""im früh-en Mor-gen-tal.""
                            [440,0.5],[370,0.5],
                            [330,1.5],
                        ];
                        const playOnce = () => {
                            let t = ctxAudio.currentTime + 0.05;
                            for (const [freq, beats] of melody) {
                                const osc = ctxAudio.createOscillator();
                                const gain = ctxAudio.createGain();
                                gain.gain.setValueAtTime(0.08, t);
                                gain.gain.exponentialRampToValueAtTime(0.0001, t + beats * beat);
                                osc.frequency.value = freq;
                                osc.type = 'sine';
                                osc.connect(gain).connect(ctxAudio.destination);
                                osc.start(t);
                                osc.stop(t + beats * beat);
                                t += beats * beat;
                            }
                            return t - ctxAudio.currentTime;
                        };
                        const loop = () => {
                            const dur = playOnce();
                            setTimeout(loop, Math.max(0, dur * 1000));
                        };
                        ctxAudio.resume().then(() => loop()).catch(() => {});
                        window.__maiLoop = true;
                    }
                }

                const resize = () => {
                    canvas.width = window.innerWidth;
                    canvas.height = window.innerHeight;
                };
                resize();
                window.addEventListener('resize', resize);

                // Maibäume (Hintergrund)
                const maypoles = Array.from({ length: 4 }, (_, i) => ({
                    x: (canvas.width / 4) * i + canvas.width / 8,
                    phase: (i / 4) * Math.PI * 2,
                }));

                // Gartenzwerge – tanzen von unten herein
                const hatColors = ['#cc0000','#0044cc','#228B22','#8B0000','#FF6600','#9900cc'];
                const gnomes = Array.from({ length: 6 }, (_, i) => ({
                    x: (canvas.width / 6) * i + canvas.width / 12,
                    y: canvas.height + 160,
                    targetY: canvas.height - 160 - (i % 3) * 35,
                    phase: (i / 6) * Math.PI * 2,
                    riseSpeed: 0.8 + (i % 3) * 0.2,
                    hatColor: hatColors[i],
                }));

                // Blüten und Blätter steigen von unten auf
                const particles = Array.from({ length: 35 }, () => ({
                    x: Math.random() * canvas.width,
                    y: canvas.height + Math.random() * 300,
                    speed: 0.4 + Math.random() * 0.9,
                    sway: Math.random() * Math.PI * 2,
                    size: 5 + Math.random() * 9,
                    color: `hsl(${Math.random() * 120 + 80}, 80%, 55%)`,
                    isFlower: Math.random() > 0.4,
                    rot: Math.random() * Math.PI * 2,
                }));

                const drawMaypole = (x, t) => {
                    const poleH = 220, groundY = canvas.height - 10;
                    const cols = ['#cc0000','#00aa44','#0055cc','#ffcc00','#cc00aa'];
                    ctx.save();
                    ctx.beginPath(); ctx.moveTo(x, groundY); ctx.lineTo(x, groundY - poleH);
                    ctx.strokeStyle = '#8B4513'; ctx.lineWidth = 9; ctx.stroke();
                    for (let s = 0; s < 5; s++) {
                        ctx.beginPath();
                        ctx.moveTo(x - 4, groundY - s * 42);
                        ctx.lineTo(x + 4, groundY - s * 42 - 21);
                        ctx.strokeStyle = cols[s]; ctx.lineWidth = 4; ctx.stroke();
                    }
                    for (let r = 0; r < 5; r++) {
                        const base = (r / 5) * Math.PI * 2;
                        const angle = base + Math.sin(t * 1.5 + base) * 0.35;
                        const len = 90;
                        const cp1x = x + Math.cos(angle) * 50;
                        const cp1y = groundY - poleH + 40;
                        const endX = x + Math.cos(angle) * len;
                        const endY = groundY - poleH + 10 + len * 0.85;
                        ctx.beginPath(); ctx.moveTo(x, groundY - poleH);
                        ctx.quadraticCurveTo(cp1x, cp1y, endX, endY);
                        ctx.strokeStyle = cols[r]; ctx.lineWidth = 3; ctx.stroke();
                    }
                    ctx.beginPath(); ctx.arc(x, groundY - poleH, 13, 0, Math.PI * 2);
                    ctx.fillStyle = '#ffdd00'; ctx.fill();
                    ctx.strokeStyle = '#cc8800'; ctx.lineWidth = 2; ctx.stroke();
                    ctx.restore();
                };

                const drawGnome = (x, y, phase, hatColor) => {
                    const sway = Math.sin(phase) * 14;
                    const lift = Math.abs(Math.sin(phase * 1.5)) * 12;
                    ctx.save();
                    ctx.translate(x + sway, y - lift);
                    ctx.rotate(sway * 0.025);
                    // Schuhe
                    ctx.fillStyle = '#222';
                    ctx.beginPath(); ctx.ellipse(-10, 52, 8, 5, -0.2, 0, Math.PI * 2); ctx.fill();
                    ctx.beginPath(); ctx.ellipse(10, 52, 8, 5, 0.2, 0, Math.PI * 2); ctx.fill();
                    // Beine
                    ctx.lineWidth = 8; ctx.lineCap = 'round'; ctx.strokeStyle = '#333355';
                    ctx.beginPath(); ctx.moveTo(-8, 22); ctx.lineTo(-10, 48 + Math.sin(phase) * 7); ctx.stroke();
                    ctx.beginPath(); ctx.moveTo(8, 22); ctx.lineTo(10, 48 - Math.sin(phase) * 7); ctx.stroke();
                    // Körper
                    ctx.beginPath(); ctx.ellipse(0, 2, 20, 24, 0, 0, Math.PI * 2);
                    ctx.fillStyle = '#3a8a3a'; ctx.fill();
                    ctx.strokeStyle = '#256025'; ctx.lineWidth = 1.5; ctx.stroke();
                    // Gürtel
                    ctx.fillStyle = '#8B6914'; ctx.fillRect(-14, 8, 28, 9);
                    ctx.fillStyle = '#ffcc00'; ctx.fillRect(-6, 9, 12, 7);
                    // Arme im Walzertakt
                    ctx.lineWidth = 7; ctx.lineCap = 'round'; ctx.strokeStyle = '#c68a56';
                    ctx.beginPath(); ctx.moveTo(-20, -8); ctx.lineTo(-38, -8 + Math.sin(phase + Math.PI) * 14); ctx.stroke();
                    ctx.beginPath(); ctx.moveTo(20, -8); ctx.lineTo(38, -8 + Math.sin(phase) * 14); ctx.stroke();
                    ctx.fillStyle = '#ffe0b2';
                    ctx.beginPath(); ctx.arc(-38, -8 + Math.sin(phase + Math.PI) * 14, 5, 0, Math.PI * 2); ctx.fill();
                    ctx.beginPath(); ctx.arc(38, -8 + Math.sin(phase) * 14, 5, 0, Math.PI * 2); ctx.fill();
                    // Kopf
                    ctx.beginPath(); ctx.arc(0, -30, 16, 0, Math.PI * 2);
                    ctx.fillStyle = '#ffe0b2'; ctx.fill();
                    ctx.strokeStyle = '#d4a47c'; ctx.lineWidth = 1; ctx.stroke();
                    // Bart
                    ctx.beginPath(); ctx.ellipse(0, -18, 12, 10, 0, 0, Math.PI);
                    ctx.fillStyle = '#f8f8f8'; ctx.fill();
                    // Augen
                    ctx.fillStyle = '#333';
                    ctx.beginPath(); ctx.arc(-5, -33, 2.5, 0, Math.PI * 2); ctx.fill();
                    ctx.beginPath(); ctx.arc(5, -33, 2.5, 0, Math.PI * 2); ctx.fill();
                    // Nase
                    ctx.beginPath(); ctx.arc(0, -26, 3.5, 0, Math.PI * 2);
                    ctx.fillStyle = '#f4a380'; ctx.fill();
                    // Spitzmütze
                    ctx.beginPath(); ctx.moveTo(-16, -40); ctx.lineTo(2, -78); ctx.lineTo(18, -40); ctx.closePath();
                    ctx.fillStyle = hatColor; ctx.fill();
                    ctx.strokeStyle = '#222'; ctx.lineWidth = 1; ctx.stroke();
                    ctx.beginPath(); ctx.ellipse(0, -40, 18, 6, 0, 0, Math.PI * 2);
                    ctx.fillStyle = hatColor; ctx.fill();
                    ctx.strokeStyle = '#222'; ctx.stroke();
                    ctx.restore();
                };

                const drawFlower = (x, y, size, color, rot) => {
                    ctx.save(); ctx.translate(x, y); ctx.rotate(rot);
                    for (let p = 0; p < 6; p++) {
                        ctx.beginPath();
                        ctx.arc(Math.cos((p / 6) * Math.PI * 2) * size, Math.sin((p / 6) * Math.PI * 2) * size, size * 0.5, 0, Math.PI * 2);
                        ctx.fillStyle = color; ctx.fill();
                    }
                    ctx.beginPath(); ctx.arc(0, 0, size * 0.45, 0, Math.PI * 2);
                    ctx.fillStyle = '#ffff88'; ctx.fill();
                    ctx.restore();
                };

                const drawLeaf = (x, y, size, color, rot) => {
                    ctx.save(); ctx.translate(x, y); ctx.rotate(rot);
                    ctx.beginPath(); ctx.ellipse(0, 0, size, size * 0.45, 0, 0, Math.PI * 2);
                    ctx.fillStyle = color; ctx.fill();
                    ctx.restore();
                };

                let frame = 0;
                const draw = () => {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);
                    const t = frame / 60;

                    for (const mp of maypoles) drawMaypole(mp.x, t + mp.phase);

                    for (const p of particles) {
                        p.y -= p.speed;
                        p.sway += 0.018;
                        p.x += Math.sin(p.sway) * 0.7;
                        p.rot += 0.01;
                        if (p.y < -20) { p.y = canvas.height + 20; p.x = Math.random() * canvas.width; }
                        p.isFlower ? drawFlower(p.x, p.y, p.size, p.color, p.rot)
                                   : drawLeaf(p.x, p.y, p.size, p.color, p.rot);
                    }

                    for (const g of gnomes) {
                        if (g.y > g.targetY) g.y -= g.riseSpeed;
                        g.phase += 0.038;
                        drawGnome(g.x, g.y, g.phase, g.hatColor);
                    }

                    frame++;
                    requestAnimationFrame(draw);
                };
                draw();
            }
        ");

        if (localDemoMode)
        {
            await page.PauseAsync();
        }
        await browser.CloseAsync();
    }
    #endregion
}
