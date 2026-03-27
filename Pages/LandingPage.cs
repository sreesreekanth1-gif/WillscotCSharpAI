using Allure.Net.Commons;
using Framework.Utilities;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Framework.Pages
{
    public class LandingPage : BasePage
    {
        private readonly string _logoSelector = "svg[aria-label='willscot logo small icon']";
        private readonly string _navItemsSelector = "nav li";
        private readonly string _searchSelector = "input[name='q'], input[type='search']";

        public LandingPage(IPage page) : base(page) { }

        public async Task<bool> IsLogoDisplayedAsync()
        {
            return await AllureApi.Step("Verify logo is displayed", async () =>
            {
                var logo = await SelfHealingLocator.ResolveAsync(Page, _logoSelector);
                return await logo.First.IsVisibleAsync();
            });
        }

        public async Task<int> GetNavigationCountAsync()
        {
            return await AllureApi.Step("Get navigation menu item count", () =>
                Page.Locator(_navItemsSelector).CountAsync());
        }

        public override async Task<string> GetPageTitleAsync()
        {
            return await AllureApi.Step("Get page title", () =>
                base.GetPageTitleAsync());
        }

        public async Task<bool> IsSearchBoxPresentAsync()
        {
            return await AllureApi.Step("Check search box is present", async () =>
            {
                var locator = Page.Locator(_searchSelector);
                return await locator.CountAsync() > 0;
            });
        }
    }
}
