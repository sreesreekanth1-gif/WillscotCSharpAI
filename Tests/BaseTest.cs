using Allure.Commons;
using Allure.NUnit;
using Framework.Configuration;
using Framework.Utilities;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Framework.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    [AllureNUnit]
    public abstract class BaseTest
    {
        protected IBrowser? Browser;
        protected IPlaywright? PlaywrightInstance;
        protected IBrowserContext? BrowserContext;
        protected IPage? Page;
        private Stopwatch _stopwatch = null!;

        private static int _totalTests;
        private static int _passedTests;
        private static int _failedTests;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        [SetUp]
        public async Task SetUpAsync()
        {
            PlaywrightInstance = await Playwright.CreateAsync();

            var browserName = (Environment.GetEnvironmentVariable("BROWSER") ?? "chrome").Trim().ToLowerInvariant();

            Browser = browserName switch
            {
                "chrome"  => await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                             {
                                 Headless = true, Timeout = 60000,
                                 Args = new[] { "--disable-blink-features=AutomationControlled" }
                             }),
                "edge"    => await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                             {
                                 Headless = true, Timeout = 60000, Channel = "msedge",
                                 Args = new[] { "--disable-blink-features=AutomationControlled" }
                             }),
                "firefox" => await PlaywrightInstance.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                             {
                                 Headless = true, Timeout = 60000
                             }),
                "safari"  => await PlaywrightInstance.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                             {
                                 Headless = true, Timeout = 60000
                             }),
                "webkit"  => await PlaywrightInstance.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                             {
                                 Headless = true, Timeout = 60000
                             }),
                _         => throw new ArgumentException($"Unsupported browser: '{browserName}'. Valid values: chrome, edge, firefox, safari.")
            };

            TestContext.Progress.WriteLine($"[Browser] Running on: {browserName}");

            Allure.Net.Commons.AllureLifecycle.Instance.UpdateTestCase(tc =>
            {
                tc.parameters.Add(new Allure.Net.Commons.Parameter { name = "Browser", value = browserName });
                tc.parameters.Add(new Allure.Net.Commons.Parameter { name = "Browser Version", value = Browser!.Version });
            });

            BrowserContext = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36"
            });
            Page = await BrowserContext.NewPageAsync();
            await Page.AddInitScriptAsync("Object.defineProperty(navigator, 'webdriver', { get: () => undefined })");
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            _totalTests++;
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            if (status == TestStatus.Passed) _passedTests++;
            if (status == TestStatus.Failed) _failedTests++;

            if (status == TestStatus.Failed && Page != null)
            {
                try
                {
                    var name = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var snapshotPath = $"Reports/{name}";
                    await Page.ScreenshotAsync(new PageScreenshotOptions { Path = snapshotPath });
                    AllureLifecycle.Instance.AddAttachment(name, "image/png", snapshotPath);
                }
                catch { /* screenshot is best-effort */ }
            }

            if (BrowserContext != null)
                await BrowserContext.CloseAsync();

            if (Browser != null)
                await Browser.CloseAsync();

            PlaywrightInstance?.Dispose();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDownAsync()
        {
            _stopwatch.Stop();
            var duration = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            var reportUrl = "Reports/index.html";

            NotificationService.SendEmail(_totalTests, _passedTests, _failedTests, duration, reportUrl);
            await NotificationService.SendSlackMessage(_totalTests, _passedTests, _failedTests, duration, reportUrl);
            await NotificationService.SendTeamsMessage(_totalTests, _passedTests, _failedTests, duration, reportUrl);
        }
    }
}
