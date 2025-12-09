using Microsoft.Identity.Client;
using Microsoft.Playwright;

namespace PlayDemo;

[TestClass]
[TestCategory("MSTest")]
[TestCategory("CICD")]
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

    #region let it snow
    [TestMethod]
    public async Task ITT_NetworkRequest_Full_LetItSnowTest()
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

        await page.EvaluateAsync(@"
            () => {
                if (document.getElementById('snow-canvas')) {
                    return;
                }

                const canvas = document.createElement('canvas');
                canvas.id = 'snow-canvas';
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
                const flakes = [];
                const flakeCount = 120;

                if (!window.__jingleLoop) {
                    const AudioCtx = window.AudioContext || window.webkitAudioContext;
                    if (AudioCtx) {
                        const ctxAudio = new AudioCtx();
                        const tempo = 140; // beats per minute
                        const beat = 60 / tempo;
                        const melody = [
                            [392, 0.5], [392, 0.5], [392, 1], // G G G
                            [392, 0.5], [392, 0.5], [392, 1], // G G G
                            [392, 0.5], [494, 0.5], [330, 0.75], [370, 0.25], [392, 2], // G B E D G
                            [440, 0.5], [440, 0.5], [440, 0.75], [440, 0.25], [440, 0.5], [392, 0.5], [392, 0.5], [392, 0.25], [392, 0.25], [392, 0.5], [370, 0.5], [370, 0.5], [392, 0.5], [370, 1], [494, 1], // A...B
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
                            const duration = playOnce();
                            setTimeout(loop, Math.max(0, duration * 1000));
                        };

                        ctxAudio.resume().then(() => loop()).catch(() => {});
                        window.__jingleLoop = true;
                    }
                }

                const resize = () => {
                    canvas.width = window.innerWidth;
                    canvas.height = window.innerHeight;
                };

                resize();
                window.addEventListener('resize', resize);

                for (let i = 0; i < flakeCount; i += 1) {
                    flakes.push({
                        x: Math.random() * canvas.width,
                        y: Math.random() * canvas.height,
                        r: Math.random() * 6 + 3,
                        d: Math.random() * flakeCount,
                        vx: (Math.random() - 0.5) * 1.5,
                        vy: Math.random() + 0.5,
                        color: `hsla(${Math.random() * 360}, 80%, 90%, 0.9)`,
                    });
                }

                let angle = 0;

                const update = () => {
                    angle += 0.01;
                    for (let i = 0; i < flakeCount; i += 1) {
                        const f = flakes[i];
                        f.y += Math.cos(angle + f.d) + f.vy + f.r * 0.3;
                        f.x += Math.sin(angle) * 0.5 + f.vx;

                        if (f.x > canvas.width || f.x < 0 || f.y > canvas.height) {
                            f.x = Math.random() * canvas.width;
                            f.y = -10;
                        }
                    }
                };

                const draw = () => {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);
                    for (let i = 0; i < flakeCount; i += 1) {
                        const f = flakes[i];
                        ctx.beginPath();
                        ctx.fillStyle = f.color;
                        ctx.arc(f.x, f.y, f.r, 0, Math.PI * 2, true);
                        ctx.fill();
                    }

                    update();
                    requestAnimationFrame(draw);
                };

                draw();
            }
        ");

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_before.png" });

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session_before.png" });
        await page.PauseAsync();

        Assert.IsTrue(
            await page.GetByText("Harald Binkle").First.IsVisibleAsync() &
            await page.GetByText("Nico Orschel").First.IsVisibleAsync());

        //await page.PauseAsync();
        await browser.CloseAsync();
    }
    #endregion
}