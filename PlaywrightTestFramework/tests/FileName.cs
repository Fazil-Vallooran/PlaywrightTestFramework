using Microsoft.Playwright.NUnit;
using PlaywrightTestFramework.Pages;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;

namespace PlaywrightTestFramework.tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : BrowserBaseTest
    {
        [Test]
        public async Task MyTest()
        {
            // Log test start information
            Logger.LogInfo("Starting login test with OTP verification");

            // Log test data
            var testData = new Dictionary<string, object>
            {
                { "Username", "8137946798" },
                { "OTP", "6,3,9,5,2,7" },
                { "Target URL", "https://www.flipkart.com/" }
            };
// Replace this line:
// Logger.LogTestData(testData);

                // With the following to log each key-value pair individually:
foreach (var kvp in testData)
            {
                Logger.LogTestData(kvp.Key, kvp.Value?.ToString() ?? string.Empty);
            };

            var loginPage = new LoginPage(Page);

            // Execute login step
            await ExecuteStepAsync("Perform Login", async () =>
            {
                Logger.LogAction("Navigate and Login", "Login form", "8137946798");
                await loginPage.LoginAsync("8137946798");
            }, "Navigate to Flipkart and perform login with username");

            // Execute OTP entry step
            await ExecuteStepAsync("Enter OTP", async () =>
            {
                Logger.LogAction("Enter OTP", "OTP input fields", "6,3,9,5,2,7");
                await loginPage.EnterOtpAsync(new[] { "6", "3", "9", "5", "2", "7" });
            }, "Enter the 6-digit OTP to complete login");

            Logger.LogInfo("Login test completed successfully");
        }
    }
}
