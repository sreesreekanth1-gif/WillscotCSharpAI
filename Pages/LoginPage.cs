using Allure.Net.Commons;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Framework.Pages
{
    public class LoginPage : BasePage
    {
        private const string SignInButtonSelector    = "button:has-text('Sign in')";
        private const string EmailInputSelector      = "input[name='email']";
        private const string PasswordInputSelector   = "input[name='password']";
        private const string LoginButtonSelector     = "button[type='submit'].auth0-lock-submit";
        private const string EmailErrorSelector      = "#auth0-lock-error-msg-email .auth0-lock-error-invalid-hint";
        private const string PasswordErrorSelector   = "#auth0-lock-error-msg-password .auth0-lock-error-invalid-hint";
        private const string GlobalErrorSelector     = ".auth0-global-message-error";

        public LoginPage(IPage page) : base(page) { }

        public async Task NavigateAndOpenLoginAsync(string baseUrl)
        {
            await AllureApi.Step("Navigate to home page and click Sign In", async () =>
            {
                await Page.GotoAsync(baseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
                await Page.Locator(SignInButtonSelector).ClickAsync();
                await Page.Locator(EmailInputSelector).WaitForAsync(new LocatorWaitForOptions { Timeout = 15000 });
            });
        }

        public Task EnterEmailAsync(string email)
        {
            return AllureApi.Step($"Enter email: '{email}'", () =>
                Page.Locator(EmailInputSelector).FillAsync(email));
        }

        public Task EnterPasswordAsync(string password)
        {
            return AllureApi.Step("Enter password", () =>
                Page.Locator(PasswordInputSelector).FillAsync(password));
        }

        public Task ClickLoginAsync()
        {
            return AllureApi.Step("Click LOG IN button", () =>
                Page.Locator(LoginButtonSelector).ClickAsync());
        }

        public async Task<string> GetEmailErrorAsync()
        {
            return await AllureApi.Step("Get email field error message", async () =>
            {
                var locator = Page.Locator(EmailErrorSelector);
                await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
                return await locator.InnerTextAsync();
            });
        }

        public async Task<string> GetPasswordErrorAsync()
        {
            return await AllureApi.Step("Get password field error message", async () =>
            {
                var locator = Page.Locator(PasswordErrorSelector);
                await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
                return await locator.InnerTextAsync();
            });
        }

        public async Task<string> GetGlobalErrorAsync()
        {
            return await AllureApi.Step("Get global error message", async () =>
            {
                var locator = Page.Locator(GlobalErrorSelector);
                await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
                return await locator.InnerTextAsync();
            });
        }

        public async Task<bool> IsEmailErrorVisibleAsync()
        {
            return await AllureApi.Step("Check email error is visible", () =>
                Page.Locator(EmailErrorSelector).IsVisibleAsync());
        }

        public async Task<bool> IsPasswordErrorVisibleAsync()
        {
            return await AllureApi.Step("Check password error is visible", () =>
                Page.Locator(PasswordErrorSelector).IsVisibleAsync());
        }

        public async Task<bool> IsGlobalErrorVisibleAsync()
        {
            return await AllureApi.Step("Check global error is visible", async () =>
            {
                try
                {
                    await Page.Locator(GlobalErrorSelector).WaitForAsync(new LocatorWaitForOptions { Timeout = 15000 });
                    return await Page.Locator(GlobalErrorSelector).IsVisibleAsync();
                }
                catch { return false; }
            });
        }
    }
}
