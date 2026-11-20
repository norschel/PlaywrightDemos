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

// howto execute: dotnet test --filter "Category=NUnit" -- NUnit.NumberOfTestWorkers=20
namespace PlaywrightDemos
{
    [Parallelizable(ParallelScope.All)]
    [Category("NUnit")]
    [TestCategory("NUnit")]
    public class AzurePlaywrightTests_KET2026_3 : CloudBrowserPageTest
    {
        private static bool _isHeadless = true;
        private static bool _isEnabledTracing = true;

        [Test]
        public async Task KET2026_SimpleSmokeTest()
        {
            await RunKETScenario("KET2026_SimpleSmokeTest");
        }

        [Test]
        public async Task KET2026_Video_SimpleSmokeTest()
        {
            await RunKETScenario("KET2026_Video_SimpleSmokeTest", recordVideo: true);
        }

        [TestCase("Chromium")]
        //[TestCase("Firefox")]
        //[TestCase("Webkit")]
        public async Task DataDriven_KET2026_SimpleSmokeTest(string browserName)
        {
            await RunKETScenario($"DataDriven_KET2026_SimpleSmokeTest_{browserName}", browserName: browserName);
        }

        private async Task RunKETScenario(string testName, string browserName = "chrome", bool recordVideo = false)
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
                await StartTrace(browserContext);

                var page = await browserContext.NewPageAsync();
                await page.GotoAsync("https://entwicklertag.de/");
                await page.Locator("text=Programm").First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Conference Day" }).ClickAsync();

                await page.Locator("text=Playwright").HighlightAsync();
                await page.Locator("text=Playwright").ScrollIntoViewIfNeededAsync();
                await page.Locator("text=Playwright").ClickAsync();

                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "session.png" });

                //Assert.IsTrue(
                //    await page.GetByText("Rheinauen").First.IsVisibleAsync(),
                //    "Rheinauen text should be visible on the page");

                var title = await page.TitleAsync();
                Assert.That(title, Does.Contain("Karlsruher Entwicklertag"), "Page title should contain 'Karlsruher Entwicklertag'");

                await StopTrace(browserContext, testName);
                await browserContext.CloseAsync();
            }
        }

        #region Helper

        public async Task StartTrace(IBrowserContext context)
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

        public static async Task StopTrace(IBrowserContext context, string testName)
        {
            if (!_isEnabledTracing)
            {
                return;
            }

            var traceOptions = new TracingStopOptions
            {
                Path = testName + "_trace.zip"
            };
            await context.Tracing.StopAsync(traceOptions);

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
