using NUnit.Framework;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;
using PlaywrightTestFramework.tests;

namespace PlaywrightTestFramework.tests
{
    [TestFixture]
    public class LoggingDebugTest : BaseTest
    {
        [Test]
        [Priority(TestPriority.High)]
        [Category("Debug")]
        [Description("Debug test to verify logging functionality")]
        public async Task DebugLoggingTest()
        {
            // Test basic logging
            Console.WriteLine("=== CONSOLE LOG TEST ===");
            TestContext.WriteLine("=== TESTCONTEXT LOG TEST ===");
            
            Logger.LogInfo("This is an info message from Logger", "Debug");
            
            // Test direct ReportPortal logging
            PlaywrightTestFramework.Reporting.ReportPortalHelper.LogInfo("This is direct ReportPortal logging");
            
            // Test step logging
            await ExecuteStepAsync("Debug logging step", async () =>
            {
                Logger.LogInfo("Inside step logging", "Step");
                Console.WriteLine("Console log inside step");
                TestContext.WriteLine("TestContext log inside step");
                
                // Simulate some test action
                await Task.Delay(100);
                
                Logger.LogTestData("Test Data Key", "Test Data Value");
                
            }, "Testing step-based logging functionality");
            
            // Test different log levels
            Logger.LogInfo("? This is an INFO message", "Test");
            Logger.LogWarn("?? This is a WARNING message", "Test");
            Logger.LogError("? This is an ERROR message", "Test");
            Logger.LogDebug("?? This is a DEBUG message", "Test");
            
            Console.WriteLine("=== END OF DEBUG TEST ===");
            TestContext.WriteLine("=== END OF DEBUG TEST ===");
        }
    }
}