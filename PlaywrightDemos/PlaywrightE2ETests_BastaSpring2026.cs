using Microsoft.Playwright;

namespace PlaywrightDemos;

[TestClass]
[TestCategory("MSTest")]
[TestCategory("CICD")]
public class BastaSpring2026_Demos
{
    #region Globals
    static bool _isHeadless = false;
    static int _slomo = 2000;
    static bool _isEnabledTracing = true;

    #endregion

    //playwright.Selectors.SetTestIdAttribute("id");

    //var tag2Label = page.GetByTestId("tag-2-label");
    //await tag2Label.ClickAsync();

    #region SimpleSmokeTest
    [TestMethod]
    public async Task BastaSpring2026_SimpleSmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
            SlowMo = _slomo,
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        Assert.Contains("Playwright", page.TitleAsync().Result);
        //await context.CloseAsync();
        //await browser.CloseAsync();
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");
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
            SlowMo = _slomo,
        };

        var browserContextOptions = new BrowserNewContextOptions
        {
            RecordVideoDir = "videos/",
            RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 }
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync(browserContextOptions);
        StartTrace(context);

        StartTrace(context);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        Assert.Contains("Playwright", page.TitleAsync().Result);
        //await context.CloseAsync();
        //await browser.CloseAsync();
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");
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
                SlowMo = _slomo,
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
    #endregion

    #region DataDriven
    [TestMethod]
    [DataTestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    public async Task DataDriven_BastaSpring2026_SimpleSmokeTest(string BrowserName)
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await GetBrowserAsync(playwright, BrowserName);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        Assert.Contains("Playwright", page.TitleAsync().Result);
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");
        await context.CloseAsync();
        await browser.CloseAsync();
    }

    private static async Task<IBrowser> GetBrowserAsync(IPlaywright playwright, string BrowserName)
    {
        var browserOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
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
            default:
                throw new ArgumentException("Browser not supported");
        }
    }
    #endregion

    #region DeviceTest
    //Temp disabled [TestMethod]
    public async Task BastaSpring2026_DeviceTest_SmokeTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
            SlowMo = _slomo
        };

        // execute test on iPhone 13 landscape
        var device = playwright.Devices["iPhone 13 landscape"];
        device.RecordVideoDir = "videos-iPhone/";
        device.RecordVideoSize = new RecordVideoSize() { Width = 1024, Height = 768 };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();
        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        Assert.Contains("Playwright", page.TitleAsync().Result);
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");

        await context.CloseAsync();
        await browser.CloseAsync();
    }
    #endregion

    #region Route Demo
    [TestMethod]
    public async Task BastaSpring2026_Playwright_RouteBlockTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
            SlowMo = _slomo,
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();

        await page.RouteAsync("**/*.jpg", route => route.FulfillAsync(new()
        {
            Status = 404,
            ContentType = "text/plain",
            Body = "Not Found!"
        }));

        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        Assert.Contains("Playwright", page.TitleAsync().Result);
        //await context.CloseAsync();
        //await browser.CloseAsync();
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");
    }
    #endregion

    #region Route Advanced

    [TestMethod]
    public async Task BastaSpring2026_NetworkRequest_FullTest()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
            SlowMo = _slomo
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();
        //https://s3.eu-west-1.amazonaws.com/redsys-prod/authors/4ddb6e4660f8176e9ec77c01/images/avatarSquareSmall_xxx_1770985900891binkleharaldwp1024x102428129.jpg
        await page.RouteAsync("**/*binkleharald*.jpg", async route =>
            {
                var response = await route.FetchAsync();

                var body = await response.BodyAsync();
                // read local image and replace the response body

                var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
                var filePath = Path.Combine(assemblyDir, "testdaten", "ostern_harry.jpg");
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
        //https://s3.eu-west-1.amazonaws.com/redsys-prod/authors/61e68dfa5642af001d4a6f30/images/avatarSquareSmall_xxx_orschel_nico_ek_1557151950696.jpg"
        await page.RouteAsync("**/*orschel_nico*.jpg", async route =>
        {
            var response = await route.FetchAsync();

            var body = await response.BodyAsync();
            // read local image and replace the response body

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "ostern_nico.jpg");
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


        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        await page.Locator("[href*=harald-binkle]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=nico-orschel]").ScrollIntoViewIfNeededAsync();

        Assert.Contains("Playwright", page.TitleAsync().Result);
        //await context.CloseAsync();
        //await browser.CloseAsync();
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");
    }

    [TestMethod]
    public async Task BastaSpring2026_NetworkRequest_FullTest_EasternTheme()
    {
        var playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless,
            SlowMo = _slomo
        };

        var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var context = await browser.NewContextAsync();
        StartTrace(context);

        var page = await context.NewPageAsync();
        //https://s3.eu-west-1.amazonaws.com/redsys-prod/authors/4ddb6e4660f8176e9ec77c01/images/avatarSquareSmall_xxx_1770985900891binkleharaldwp1024x102428129.jpg
        await page.RouteAsync("**/*binkleharald*.jpg", async route =>
            {
                var response = await route.FetchAsync();

                var body = await response.BodyAsync();
                // read local image and replace the response body

                var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
                var filePath = Path.Combine(assemblyDir, "testdaten", "ostern_harry.jpg");
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
        //https://s3.eu-west-1.amazonaws.com/redsys-prod/authors/61e68dfa5642af001d4a6f30/images/avatarSquareSmall_xxx_orschel_nico_ek_1557151950696.jpg"
        await page.RouteAsync("**/*orschel_nico*.jpg", async route =>
        {
            var response = await route.FetchAsync();

            var body = await response.BodyAsync();
            // read local image and replace the response body

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
            var filePath = Path.Combine(assemblyDir, "testdaten", "ostern_nico.jpg");
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


        await page.GotoAsync("https://basta.net/frankfurt/");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_1.png" });
        if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
        }

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_2.png" });
        await page.Locator("body").PressAsync("Escape");
        await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
        await page.Locator("body").PressAsync("Escape");

        await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=Day3]").First.HighlightAsync();
        await page.Locator("[href*=Day3]").First.ClickAsync();

        await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
        await page.Locator("[href*=playwright]").HighlightAsync();
        await page.Locator("[href*=playwright]").ClickAsync();

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot_basta_spring_2026_3.png" });

        await page.Locator("[href*=speaker/harald-binkle]").ScrollIntoViewIfNeededAsync();

        await page.EvaluateAsync(@"
            () => {
                if (document.getElementById('easter-canvas')) {
                    return;
                }

                const canvas = document.createElement('canvas');
                canvas.id = 'easter-canvas';
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

                // Easter melody: ""Häschen in der Grube"" (traditional German Easter song)
                if (!window.__easterLoop) {
                    const AudioCtx = window.AudioContext || window.webkitAudioContext;
                    if (AudioCtx) {
                        const ctxAudio = new AudioCtx();
                        const tempo = 120;
                        const beat = 60 / tempo;
                        // Häschen in der Grube: C D E C | C D E C | E F G | E F G
                        // G A G F E C | G A G F E C | C G C | C G C
                        const melody = [
                            [523, 0.5], [587, 0.5], [659, 0.5], [523, 0.5],   // C D E C
                            [523, 0.5], [587, 0.5], [659, 0.5], [523, 0.5],   // C D E C
                            [659, 0.5], [698, 0.5], [784, 1],                  // E F G
                            [659, 0.5], [698, 0.5], [784, 1],                  // E F G
                            [784, 0.375], [880, 0.125], [784, 0.25], [698, 0.25], [659, 0.5], [523, 0.5], // G A G F E C
                            [784, 0.375], [880, 0.125], [784, 0.25], [698, 0.25], [659, 0.5], [523, 0.5], // G A G F E C
                            [523, 0.5], [392, 0.5], [523, 1],                  // C G C
                            [523, 0.5], [392, 0.5], [523, 1],                  // C G C
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
                        window.__easterLoop = true;
                    }
                }

                const resize = () => {
                    canvas.width = window.innerWidth;
                    canvas.height = window.innerHeight;
                };
                resize();
                window.addEventListener('resize', resize);

                // Easter rabbit state
                const rabbit = {
                    x: -80,
                    y: canvas.height - 120,
                    frame: 0,
                    speed: 3,
                    hopPhase: 0,
                };

                // Colored Easter eggs scattered on the ground
                const eggs = [];
                for (let i = 0; i < 12; i++) {
                    eggs.push({
                        x: Math.random() * canvas.width,
                        y: canvas.height - 30 - Math.random() * 40,
                        color1: `hsl(${Math.random() * 360}, 80%, 60%)`,
                        color2: `hsl(${Math.random() * 360}, 80%, 75%)`,
                        r: 12 + Math.random() * 8,
                    });
                }

                const drawEgg = (egg) => {
                    ctx.save();
                    ctx.translate(egg.x, egg.y);
                    ctx.scale(0.75, 1);
                    ctx.beginPath();
                    ctx.arc(0, 0, egg.r, 0, Math.PI * 2);
                    ctx.fillStyle = egg.color1;
                    ctx.fill();
                    // stripe decoration
                    ctx.beginPath();
                    ctx.rect(-egg.r, -3, egg.r * 2, 6);
                    ctx.fillStyle = egg.color2;
                    ctx.fill();
                    ctx.restore();
                };

                const drawRabbit = (x, y, hopOffset) => {
                    const dy = -Math.abs(Math.sin(hopOffset)) * 30; // hop height
                    ctx.save();
                    ctx.translate(x, y + dy);

                    // Body
                    ctx.beginPath();
                    ctx.ellipse(0, 0, 28, 22, 0, 0, Math.PI * 2);
                    ctx.fillStyle = '#d2b48c';
                    ctx.fill();

                    // Head
                    ctx.beginPath();
                    ctx.arc(25, -18, 16, 0, Math.PI * 2);
                    ctx.fillStyle = '#d2b48c';
                    ctx.fill();

                    // Left ear
                    ctx.beginPath();
                    ctx.ellipse(18, -52, 5, 18, -0.2, 0, Math.PI * 2);
                    ctx.fillStyle = '#d2b48c';
                    ctx.fill();
                    ctx.beginPath();
                    ctx.ellipse(18, -52, 3, 14, -0.2, 0, Math.PI * 2);
                    ctx.fillStyle = '#f4c2c2';
                    ctx.fill();

                    // Right ear
                    ctx.beginPath();
                    ctx.ellipse(32, -50, 5, 18, 0.2, 0, Math.PI * 2);
                    ctx.fillStyle = '#d2b48c';
                    ctx.fill();
                    ctx.beginPath();
                    ctx.ellipse(32, -50, 3, 14, 0.2, 0, Math.PI * 2);
                    ctx.fillStyle = '#f4c2c2';
                    ctx.fill();

                    // Eye
                    ctx.beginPath();
                    ctx.arc(32, -20, 3, 0, Math.PI * 2);
                    ctx.fillStyle = '#333';
                    ctx.fill();

                    // Nose
                    ctx.beginPath();
                    ctx.arc(40, -15, 2.5, 0, Math.PI * 2);
                    ctx.fillStyle = '#f4a7bb';
                    ctx.fill();

                    // Tail
                    ctx.beginPath();
                    ctx.arc(-28, -2, 8, 0, Math.PI * 2);
                    ctx.fillStyle = '#fff';
                    ctx.fill();

                    // Front leg
                    const legKick = Math.sin(hopOffset * 2) * 8;
                    ctx.beginPath();
                    ctx.ellipse(18, 18 + legKick, 5, 10, 0.3, 0, Math.PI * 2);
                    ctx.fillStyle = '#c4a47a';
                    ctx.fill();

                    // Back leg
                    ctx.beginPath();
                    ctx.ellipse(-14, 16 - legKick, 7, 12, -0.3, 0, Math.PI * 2);
                    ctx.fillStyle = '#c4a47a';
                    ctx.fill();

                    ctx.restore();
                };

                const draw = () => {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);

                    // Draw eggs
                    for (const egg of eggs) {
                        drawEgg(egg);
                    }

                    // Update rabbit position
                    rabbit.hopPhase += 0.07;
                    rabbit.x += rabbit.speed;

                    // Wrap around when rabbit goes off screen
                    if (rabbit.x > canvas.width + 100) {
                        rabbit.x = -100;
                        rabbit.y = canvas.height - 100 - Math.random() * 40;
                    }

                    drawRabbit(rabbit.x, rabbit.y, rabbit.hopPhase);

                    requestAnimationFrame(draw);
                };

                draw();
            }
        ");

        Assert.Contains("Playwright", page.TitleAsync().Result);
        //await context.CloseAsync();
        //await browser.CloseAsync();
        StopTrace(context, "BastaSpring2026_SimpleSmokeTest");
    }

    //await page.RouteAsync("**/*.png", route => route.FulfillAsync(new ()
    /*{
        Status = 404,
        ContentType = "text/plain",
        Body = "Not Found!"
    }));*/

    /*await page.RouteAsync("https://basta.net/mediadaten/jobs", async route =>
    {
        var response = await route.FetchAsync();
        await route.FulfillAsync(new RouteFulfillOptions
        {
            Response = response,
            Headers = new Dictionary<string, string>(response.Headers)
            {
                ["Content-Disposition"] = "attachment"
                ,["Content-Type"] = "application/binary"
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
