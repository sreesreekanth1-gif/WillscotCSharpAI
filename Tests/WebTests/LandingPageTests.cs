using Allure.Net.Commons;
using Framework.Configuration;
using Framework.Pages;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Framework.Tests.WebTests
{
    [TestFixture]
    public class LandingPageTests : BaseTest
    {
        [Test]
        public async Task VerifyPageTitle()
        {
            var landingPage = new LandingPage(Page);
            await landingPage.NavigateToAsync(ConfigReader.BaseUrl);
            var title = await landingPage.GetPageTitleAsync();
            AllureApi.Step("Assert page title contains 'WillScot'", () =>
                Assert.That(title, Does.Contain("WillScot")));
        }

        [Test]
        public async Task VerifyLogoExists()
        {
            var landingPage = new LandingPage(Page);
            await landingPage.NavigateToAsync(ConfigReader.BaseUrl);
            var isVisible = await landingPage.IsLogoDisplayedAsync();
            AllureApi.Step("Assert logo is visible", () =>
                Assert.That(isVisible, Is.True));
        }

        [Test]
        public async Task VerifyNavigationMenu()
        {
            var landingPage = new LandingPage(Page);
            await landingPage.NavigateToAsync(ConfigReader.BaseUrl);
            var count = await landingPage.GetNavigationCountAsync();
            AllureApi.Step($"Assert navigation has at least 3 items (found {count})", () =>
                Assert.That(count, Is.GreaterThanOrEqualTo(3)));
        }
    }
}