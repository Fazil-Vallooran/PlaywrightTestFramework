using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;
using PlaywrightTestFramework.Extensions;
using NUnit.Framework;

namespace PlaywrightTestFramework.tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ExampleTest : BrowserBaseTest
    {
        [Test]
        [Order(1)]
        [Category("Smoke")]
        [Priority(TestPriority.High)]
        [Description("Verify that the Playwright website title contains 'Playwright'")]
        public async Task HasTitle()
        {
            Logger.LogInfo($"?? Test Priority: HIGH | Category: Smoke Test", "Test Info");
            Logger.LogInfo("This is a critical smoke test for basic page functionality", "Test Info");
            
            await ExecuteStepAsync("Navigate to Playwright website", async () =>
            {
                Logger.LogInfo("Navigating to https://playwright.dev", "Navigation");
                await Page.GotoAsync("https://playwright.dev");
                Logger.LogAction("Navigate", "https://playwright.dev");
            }, "Navigate to the Playwright documentation website");

            await ExecuteStepAsync("Verify page title contains 'Playwright'", async () =>
            {
                Logger.LogInfo("Verifying that page title contains 'Playwright'", "Verification");
                
                // Get actual title for logging
                var actualTitle = await Page.TitleAsync();
                Logger.LogTestData("Actual page title", actualTitle);
                
                // Expect a title "to contain" a substring.
                await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
                
                Logger.LogVerification("Title contains 'Playwright'", "Title containing 'Playwright'", actualTitle, true);
                Logger.LogInfo("? Title verification passed successfully", "Verification");
            }, "Verify that the page title contains the expected text");
        }

        [Test]
        [Order(2)]
        [Category("Functional")]
        [Priority(TestPriority.Medium)]
        [Description("Verify navigation to the Installation page via the Get Started link")]
        public async Task GetStartedLink()
        {
            Logger.LogInfo($"?? Test Priority: MEDIUM | Category: Functional Test", "Test Info");
            Logger.LogInfo("This test verifies core navigation functionality", "Test Info");
            
            await ExecuteStepAsync("Navigate to Playwright website", async () =>
            {
                Logger.LogInfo("Navigating to https://playwright.dev", "Navigation");
                await Page.GotoAsync("https://playwright.dev");
                Logger.LogAction("Navigate", "https://playwright.dev");
            }, "Navigate to the Playwright documentation website");

            await ExecuteStepAsync("Click on 'Get started' link", async () =>
            {
                Logger.LogInfo("Looking for 'Get started' link", "UI Interaction");
                
                var getStartedLink = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
                
                // Log that we found the element
                Logger.LogInfo("Found 'Get started' link, clicking it", "UI Interaction");
                
                // Click the get started link.
                await getStartedLink.ClickAsync();
                
                Logger.LogAction("Click", "Get started link");
                Logger.LogInfo("? Successfully clicked 'Get started' link", "UI Interaction");
            }, "Click on the 'Get started' link to navigate to installation page");

            await ExecuteStepAsync("Verify Installation heading is visible", async () =>
            {
                Logger.LogInfo("Verifying that 'Installation' heading is visible", "Verification");
                
                var installationHeading = Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" });
                
                // Expects page to have a heading with the name of Installation.
                await Expect(installationHeading).ToBeVisibleAsync();
                
                // Get the actual text for logging
                var headingText = await installationHeading.TextContentAsync();
                Logger.LogTestData("Installation heading text", headingText ?? "");
                
                Logger.LogVerification("Installation heading is visible", "Heading 'Installation' visible", headingText ?? "", true);
                Logger.LogInfo("? Installation heading verification passed", "Verification");
            }, "Verify that the Installation heading is visible on the page");
        }
    }

    /// <summary>
    /// Custom Priority attribute for test categorization and ReportPortal integration
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PriorityAttribute : Attribute
    {
        public TestPriority Priority { get; }
        
        public PriorityAttribute(TestPriority priority)
        {
            Priority = priority;
        }
    }

    /// <summary>
    /// Enumeration defining test priority levels
    /// </summary>
    public enum TestPriority
    {
        /// <summary>
        /// Critical tests that must pass - usually smoke tests
        /// </summary>
        Critical = 0,
        
        /// <summary>
        /// High priority tests - core functionality
        /// </summary>
        High = 1,
        
        /// <summary>
        /// Medium priority tests - important features
        /// </summary>
        Medium = 2,
        
        /// <summary>
        /// Low priority tests - nice to have features
        /// </summary>
        Low = 3,
        
        /// <summary>
        /// Optional tests - edge cases or experimental features
        /// </summary>
        Optional = 4
    }
}