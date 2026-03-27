using Allure.Net.Commons;
using Framework.Utilities;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Framework.Pages
{
    public abstract class BasePage
    {
        protected IPage Page { get; }

        protected BasePage(IPage page)
        {
            Page = page;
        }

        public Task NavigateToAsync(string url)
        {
            return AllureApi.Step($"Navigate to {url}", () =>
                Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load }));
        }

        public virtual async Task<string> GetPageTitleAsync()
        {
            return await AllureApi.Step("Get page title", () =>
                Page.TitleAsync());
        }

        public async Task<bool> IsElementVisibleAsync(string selector)
        {
            return await AllureApi.Step($"Check element is visible: {selector}", async () =>
            {
                var locator = await SelfHealingLocator.ResolveAsync(Page, selector);
                return await locator.IsVisibleAsync();
            });
        }

        public async Task TakeScreenshotAsync(string fileName)
        {
            await AllureApi.Step($"Take screenshot: {fileName}", async () =>
            {
                var screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "screenshots");
                Directory.CreateDirectory(screenshotDir);
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = Path.Combine(screenshotDir, fileName), FullPage = true });
            });
        }

        public Task WaitForElementAsync(string selector, int timeoutMs = 10000)
        {
            return AllureApi.Step($"Wait for element: {selector}", () =>
                Page.Locator(selector).WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs }));
        }
    }
}
