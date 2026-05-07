using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Ignore = NUnit.Framework.IgnoreAttribute;
using NUnit.Framework.Interfaces;
using Microsoft.Playwright.NUnit;
using Azure.Developer.Playwright;
using Azure.Identity;

namespace PlaywrightDemos
{
    //[Parallelizable(ParallelScope.Self)]
    public class AzurePlaywrightTests_BastaSpring2026_3
     : CloudBrowserPageTest
    {
        private static bool _isHeadless = true;
        private static bool _isEnabledTracing = true;

        [Test]
        public async Task BastaSpring2026_SimpleSmokeTest()
        {
            await RunBastaScenario("BastaSpring2026_SimpleSmokeTest");
        }

        [Test]
        public async Task BastaSpring2026_Video_SimpleSmokeTest()
        {
            await RunBastaScenario("BastaSpring2026_Video_SimpleSmokeTest", recordVideo: true);
        }

        [TestCase("Chromium")]
        //[TestCase("Firefox")]
        //[TestCase("Webkit")]
        public async Task DataDriven_BastaSpring2026_SimpleSmokeTest(string browserName)
        {
            await RunBastaScenario($"DataDriven_BastaSpring2026_SimpleSmokeTest_{browserName}", browserName: browserName);
        }

        private async Task RunBastaScenario(string testName, string browserName = "chrome", bool recordVideo = false)
        {
            var playwright = this.Playwright;

            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = _isHeadless
            };

            IBrowser browser;
            switch (browserName)
            {
                case "Firefox":
                    browser = await playwright.Firefox.LaunchAsync(launchOptions);
                    break;
                case "Webkit":
                    browser = await playwright.Webkit.LaunchAsync(launchOptions);
                    break;
                default:
                    browser = await playwright.Chromium.LaunchAsync(launchOptions);
                    break;
            }

            var contextOptions = new BrowserNewContextOptions();
            if (recordVideo)
            {
                contextOptions.RecordVideoDir = "videos/";
                contextOptions.RecordVideoSize = new RecordVideoSize { Width = 1024, Height = 768 };
            }

            await using (browser)
            {
                var browserContext = await browser.NewContextAsync(contextOptions);
                StartTrace(browserContext);

                var page = await browserContext.NewPageAsync();
                await page.GotoAsync("https://basta.net/frankfurt/");

                if (await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).IsVisibleAsync())
                {
                    await page.GetByRole(AriaRole.Button, new() { Name = "Alle akzeptieren" }).ClickAsync();
                }

                await page.Locator("body").PressAsync("Escape");
                await page.GetByRole(AriaRole.Link, new() { Name = "Programm" }).First.ClickAsync();
                await page.Locator("body").PressAsync("Escape");

                await page.Locator("[href*=Day3]").First.ScrollIntoViewIfNeededAsync();
                await page.Locator("[href*=Day3]").First.HighlightAsync();
                await page.Locator("[href*=Day3]").First.ClickAsync();

                await page.Locator("[href*=playwright]").ScrollIntoViewIfNeededAsync();
                await page.Locator("[href*=playwright]").HighlightAsync();
                await page.Locator("[href*=playwright]").ClickAsync();

                Assert.IsTrue((await page.TitleAsync()).Contains("Playwright"));

                StopTrace(browserContext, testName);
                await browserContext.CloseAsync();
            }
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
            };
            context.Tracing.StopAsync(traceOptions).Wait();

            try
            {
                var tracePath = Path.Combine(
                        NUnit.Framework.TestContext.CurrentContext.WorkDirectory,
                        $"{testName}_trace.zip"
                    );
                NUnit.Framework.TestContext.AddTestAttachment(
                    $"{testName}_trace.zip",
                    $"{testName}_trace.zip"
                );
            }
            catch
            {
                // ignore attachment errors in CI
            }
        }

        #endregion
    }
}
