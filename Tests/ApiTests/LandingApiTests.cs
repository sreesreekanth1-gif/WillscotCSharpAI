using Allure.Net.Commons;
using Framework.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Tests.ApiTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class LandingApiTests : BaseTest
    {
        [Test]
        public async Task VerifyLandingPageReturns200()
        {
            var endpoint = ConfigReader.ApiEndpoints().LandingPage;

            using var playwright = await Playwright.CreateAsync();
            IAPIRequestContext apiContext = null!;
            await AllureApi.Step("Create API request context with browser headers", async () =>
            {
                apiContext = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
                {
                    ExtraHTTPHeaders = new Dictionary<string, string>
                    {
                        ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36",
                        ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
                    }
                });
            });

            IAPIResponse response = null!;
            await AllureApi.Step($"GET {endpoint}", async () =>
            {
                response = await apiContext.GetAsync(endpoint);
            });

            AllureApi.Step($"Assert response status is 200 (actual: {(int)response.Status})", () =>
                Assert.That((int)response.Status, Is.EqualTo(200)));

            var body = await AllureApi.Step("Read response body", async () =>
                await response.TextAsync());

            AllureApi.Step("Assert response body contains 'WillScot'", () =>
                Assert.That(body, Does.Contain("WillScot")));
        }
    }
}
