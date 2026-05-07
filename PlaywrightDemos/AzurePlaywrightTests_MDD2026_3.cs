using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework.Interfaces;
using Microsoft.Playwright.NUnit;
using Azure.Developer.Playwright;
using Azure.Identity;

namespace PlaywrightDemos
{
    [Parallelizable(ParallelScope.Self)]
    [Category("NUnit")]
    [TestCategory("NUnit")]
    public class AzurePlaywrightTests_MDD2026_3 : CloudBrowserPageTest
    {
        private static bool _isHeadless = true;
        private static bool _isEnabledTracing = true;

        [Test]
        public async Task MDD2026_SimpleSmokeTest()
        {
            await RunMDDScenario("MDD2026_SimpleSmokeTest");
        }

        [Test]
        public async Task MDD2026_Video_SimpleSmokeTest()
        {
            await RunMDDScenario("MDD2026_Video_SimpleSmokeTest", recordVideo: true);
        }

        [TestCase("Chromium")]
        //[TestCase("Firefox")]
        //[TestCase("Webkit")]
        public async Task DataDriven_MDD2026_SimpleSmokeTest(string browserName)
        {
            await RunMDDScenario($"DataDriven_MDD2026_SimpleSmokeTest_{browserName}", browserName: browserName);
        }

        private async Task RunMDDScenario(string testName, string browserName = "chrome", bool recordVideo = false)
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

                Assert.IsTrue(
                    await page.GetByText("Der Workshop \"Testautomatisierung mit Playwright\" bietet eine umfassende Einführung").IsVisibleAsync());

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
