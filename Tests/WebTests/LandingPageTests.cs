using Framework.Configuration;
using Framework.Pages;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Framework.Tests.WebTests
{
    [TestFixture]
    public class LandingPageTests : BaseTest
    {
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task VerifyPageTitle()
        {
            try
            {
                var landingPage = new LandingPage(Page);
                await landingPage.NavigateToAsync(ConfigReader.BaseUrl);
                var title = await landingPage.GetPageTitleAsync();
                await StepWithScreenshotAsync("Assert page title contains 'WillScot'", () =>
                    Assert.That(title, Does.Contain("WillScot")));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task VerifyLogoExists()
        {
            try
            {
                var landingPage = new LandingPage(Page);
                await landingPage.NavigateToAsync(ConfigReader.BaseUrl);
                var isVisible = await landingPage.IsLogoDisplayedAsync();
                await StepWithScreenshotAsync("Assert logo is visible", () =>
                    Assert.That(isVisible, Is.True));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task VerifyNavigationMenu()
        {
            try
            {
                var landingPage = new LandingPage(Page);
                await landingPage.NavigateToAsync(ConfigReader.BaseUrl);
                var count = await landingPage.GetNavigationCountAsync();
                await StepWithScreenshotAsync($"Assert navigation has at least 3 items (found {count})", () =>
                    Assert.That(count, Is.GreaterThanOrEqualTo(3)));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }
    }
}