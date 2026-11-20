using Microsoft.Playwright;
using NUnit.Framework;
using PlayDemo;
using TestContext = NUnit.Framework.TestContext;

namespace PlaywrightDemos
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    [Category("NUnit")]
    public class AzurePlaywrightTests_DDC2026 : CloudBrowserPageTest
    {
        [Test]
        public async Task DDC2026_SimpleSmokeTest()
        {
            await RunDDCScenario(Browser, nameof(DDC2026_SimpleSmokeTest));
        }

        [Test]
        public async Task DDC2026_Video_SimpleSmokeTest()
        {
            await RunDDCScenario(Browser, nameof(DDC2026_Video_SimpleSmokeTest), recordVideo: true);
        }

        [TestCase("chromium")]
        //[TestCase("firefox")]
        //[TestCase("webkit")]
        public async Task DataDriven_DDC2026_SimpleSmokeTest(string browserName)
        {
            var testName = $"{nameof(DataDriven_DDC2026_SimpleSmokeTest)}_{browserName}";
            if (browserName == BrowserName)
            {
                await RunDDCScenario(Browser, testName);
                return;
            }

            var connection = await ConnectOptionsAsync()
                ?? throw new InvalidOperationException("Azure Playwright connection options are required.");
            await using var browser = await Playwright[browserName].ConnectAsync(connection.Item1, connection.Item2);
            await RunDDCScenario(browser, testName);
        }

        private async Task RunDDCScenario(IBrowser browser, string testName, bool recordVideo = false)
        {
            var artifactDirectory = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "TestResults",
                GetType().Name,
                testName);
            Directory.CreateDirectory(artifactDirectory);

            var contextOptions = new BrowserNewContextOptions();
            if (recordVideo)
            {
                contextOptions.RecordVideoDir = Path.Combine(artifactDirectory, "videos");
                contextOptions.RecordVideoSize = new RecordVideoSize { Width = 1024, Height = 768 };
            }

            await using var browserContext = await browser.NewContextAsync(contextOptions);
            var page = await browserContext.NewPageAsync();
            await browserContext.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            try
            {
                await PlaywrightE2ETests_DDC2026.OpenPlaywrightSessionAsync(page);
                await PlaywrightE2ETests_DDC2026.AssertSessionModalAsync(page);

                var screenshotPath = Path.Combine(artifactDirectory, $"{testName}.png");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                TestContext.AddTestAttachment(screenshotPath);
            }
            finally
            {
                try
                {
                    var tracePath = Path.Combine(artifactDirectory, $"{testName}_trace.zip");
                    await browserContext.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
                    TestContext.AddTestAttachment(tracePath);
                }
                finally
                {
                    await browserContext.CloseAsync();
                    if (recordVideo)
                    {
                        var video = page.Video
                            ?? throw new InvalidOperationException("The page did not produce the requested video recording.");
                        var videoPath = Path.Combine(artifactDirectory, "videos", $"{testName}.webm");
                        // Remote recordings must be downloaded after the context closes.
                        await video.SaveAsAsync(videoPath);
                        TestContext.AddTestAttachment(videoPath);
                    }
                }
            }
        }
    }
}
