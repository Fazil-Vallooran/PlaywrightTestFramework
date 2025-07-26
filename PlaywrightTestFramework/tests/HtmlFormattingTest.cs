using NUnit.Framework;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;
using PlaywrightTestFramework.tests;

namespace PlaywrightTestFramework.tests
{
    [TestFixture]
    public class HtmlFormattingTest : BaseTest
    {
        [Test]
        [Priority(TestPriority.Critical)]
        [Category("Demo")]
        [Description("Test to demonstrate improved HTML formatting in ReportPortal")]
        public async Task HtmlFormattingDemo()
        {
            // Test the new HTML-based priority icons
            Logger.LogInfo("Testing HTML-based priority icons and formatting", "Demo");
            
            await ExecuteStepAsync("Demonstrate enhanced step formatting", async () =>
            {
                Logger.LogInfo("This step demonstrates the new HTML formatting with colored boxes and borders", "Demo");
                
                // Log various types of messages
                Logger.LogInfo("? This is an info message with HTML formatting", "Info Demo");
                Logger.LogWarn("?? This is a warning message with HTML formatting", "Warning Demo");
                Logger.LogTestData("Sample Data", "This demonstrates test data formatting");
                
                // Simulate some test action
                await Task.Delay(100);
                
            }, "Testing the enhanced HTML formatting for better ReportPortal visualization");
            
            Logger.LogInfo("HTML formatting demo completed successfully", "Demo");
        }
    }
}