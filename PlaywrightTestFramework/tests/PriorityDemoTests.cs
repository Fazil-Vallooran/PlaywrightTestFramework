using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;
using PlaywrightTestFramework.tests;
using NUnit.Framework;
using System.Text.RegularExpressions;
using ReportPortal.Client.Abstractions.Models;

namespace PlaywrightTestFramework.tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class PriorityDemoTests : BaseTest
    {
        [Test]
        [Order(1)]
        [Category("Smoke")]
        [Priority(TestPriority.Critical)]
        [Description("Critical smoke test - basic page load functionality")]
        public async Task CriticalTest_PageLoads()
        {
            // Demonstrate all log levels for ReportPortal filtering
            LogAllLevels("Critical Test: Page Load Verification", "Critical Test Start");
            
            await ExecuteStepAsync("Navigate to website", async () =>
            {
                // TRACE level - detailed execution flow
                LogWithLevel("Starting navigation to Playwright website", LogLevel.Trace, "Navigation Flow");
                
                // DEBUG level - detailed debugging information
                LogWithLevel("Browser instance ready, initiating page navigation", LogLevel.Debug, "Navigation Debug");
                
                // INFO level - general information
                LogWithLevel("Navigating to https://playwright.dev", LogLevel.Info, "Navigation");
                await Page.GotoAsync("https://playwright.dev");
                Logger.LogAction("Navigate", "https://playwright.dev");
                
                // INFO level - successful completion
                LogWithLevel("Navigation completed successfully", LogLevel.Info, "Navigation Result");
                
            }, "Basic page navigation test with comprehensive logging");

            await ExecuteStepAsync("Verify page is responsive", async () =>
            {
                // TRACE level - entering verification phase
                LogWithLevel("Entering page responsiveness verification phase", LogLevel.Trace, "Verification Flow");
                
                // DEBUG level - detailed verification steps
                LogWithLevel("Checking page URL pattern against expected regex", LogLevel.Debug, "URL Verification");
                
                await Expect(Page).ToHaveURLAsync(new Regex("playwright\\.dev"));
                
                // INFO level - verification success
                LogWithLevel("? Page loaded successfully and URL verified", LogLevel.Info, "Verification");
                
                // Additional verification with different log levels
                var currentUrl = Page.Url;
                LogWithLevel($"Current page URL: {currentUrl}", LogLevel.Debug, "URL Details");
                LogWithLevel("Page responsiveness verification completed", LogLevel.Trace, "Verification Flow");
                
            }, "Ensure page loaded correctly with detailed logging");
            
            // Log test completion with INFO level
            LogWithLevel("?? CRITICAL TEST COMPLETED: All verifications passed", LogLevel.Info, "Test Result");
        }

        [Test]
        [Order(2)]
        [Category("Core")]
        [Priority(TestPriority.High)]
        [Description("High priority test - core navigation functionality")]
        public async Task HighPriorityTest_Navigation()
        {
            // WARNING level - important test starting
            LogWithLevel("?? HIGH PRIORITY TEST: Core navigation functionality starting", LogLevel.Warning, "Priority Alert");
            
            await ExecuteStepAsync("Navigate and verify title", async () =>
            {
                // TRACE level - detailed execution tracking
                LogWithLevel("Beginning title verification workflow", LogLevel.Trace, "Workflow");
                
                // DEBUG level - navigation details
                LogWithLevel("Initiating page navigation for title verification", LogLevel.Debug, "Navigation");
                
                await Page.GotoAsync("https://playwright.dev");
                
                // DEBUG level - getting page title
                var actualTitle = await Page.TitleAsync();
                LogWithLevel($"Retrieved page title: '{actualTitle}'", LogLevel.Debug, "Title Data");
                
                // INFO level - performing assertion
                LogWithLevel("Performing title assertion against regex pattern", LogLevel.Info, "Assertion");
                await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
                
                // INFO level - successful verification
                LogWithLevel("? Title verification passed successfully", LogLevel.Info, "Verification");
                
                // TRACE level - workflow completion
                LogWithLevel("Title verification workflow completed", LogLevel.Trace, "Workflow");
                
            }, "Core title verification functionality with multi-level logging");
        }

        [Test]
        [Order(3)]
        [Category("Functional")]
        [Priority(TestPriority.Medium)]
        [Description("Medium priority test - additional feature verification")]
        public async Task MediumPriorityTest_Documentation()
        {
            // INFO level - test start
            LogWithLevel("?? MEDIUM PRIORITY TEST: Documentation verification starting", LogLevel.Info, "Test Start");
            
            await ExecuteStepAsync("Verify documentation link", async () =>
            {
                // TRACE level - method entry
                LogWithLevel("Entering documentation link verification method", LogLevel.Trace, "Method Entry");
                
                await Page.GotoAsync("https://playwright.dev");
                
                // DEBUG level - element location
                LogWithLevel("Locating documentation links on page", LogLevel.Debug, "Element Location");
                var docsLink = Page.Locator("text=Docs");
                
                // DEBUG level - counting elements
                LogWithLevel("Counting documentation link elements", LogLevel.Debug, "Element Count");
                var count = await docsLink.CountAsync();
                
                // INFO level - count result
                LogWithLevel($"Found {count} documentation links on page", LogLevel.Info, "Element Count Result");
                
                if (count > 0)
                {
                    // INFO level - positive result
                    LogWithLevel("? Documentation link found - feature available", LogLevel.Info, "Feature Verification");
                    
                    // DEBUG level - additional details
                    LogWithLevel("Documentation accessibility confirmed", LogLevel.Debug, "Accessibility Check");
                }
                else
                {
                    // WARNING level - missing feature (non-critical)
                    LogWithLevel("?? Documentation link not found - non-critical for medium priority", LogLevel.Warning, "Missing Feature");
                    
                    // DEBUG level - investigation note
                    LogWithLevel("Page structure may have changed - investigate if needed", LogLevel.Debug, "Investigation Note");
                }
                
                // TRACE level - method exit
                LogWithLevel("Exiting documentation link verification method", LogLevel.Trace, "Method Exit");
                
            }, "Documentation accessibility check with comprehensive logging");
        }

        [Test]
        [Order(4)]
        [Category("Enhancement")]
        [Priority(TestPriority.Low)]
        [Description("Low priority test - nice-to-have feature")]
        public async Task LowPriorityTest_SocialLinks()
        {
            // INFO level - low priority test start
            LogWithLevel("?? LOW PRIORITY TEST: Social media presence verification", LogLevel.Info, "Enhancement Test");
            
            await ExecuteStepAsync("Check for social media links", async () =>
            {
                // TRACE level - detailed flow
                LogWithLevel("Starting social media link detection process", LogLevel.Trace, "Social Media Flow");
                
                await Page.GotoAsync("https://playwright.dev");
                
                // DEBUG level - search strategy
                LogWithLevel("Searching for GitHub links using href attribute pattern", LogLevel.Debug, "Search Strategy");
                var githubLink = Page.Locator("a[href*='github']");
                
                // DEBUG level - element counting
                LogWithLevel("Executing element count for GitHub links", LogLevel.Debug, "Element Counting");
                var count = await githubLink.CountAsync();
                
                // INFO level - results
                LogWithLevel($"Social media scan result: {count} GitHub links detected", LogLevel.Info, "Scan Results");
                
                if (count > 0)
                {
                    // INFO level - feature present
                    LogWithLevel("? GitHub social media integration confirmed", LogLevel.Info, "Social Integration");
                    
                    // DEBUG level - enhancement details
                    LogWithLevel("Social media presence enhances project credibility", LogLevel.Debug, "Enhancement Value");
                }
                else
                {
                    // WARNING level - missing enhancement (acceptable for low priority)
                    LogWithLevel("?? GitHub links not detected - acceptable for low priority enhancement", LogLevel.Warning, "Enhancement Gap");
                    
                    // DEBUG level - impact assessment
                    LogWithLevel("Missing social links have minimal impact on core functionality", LogLevel.Debug, "Impact Assessment");
                }
                
                // TRACE level - process completion
                LogWithLevel("Social media link detection process completed", LogLevel.Trace, "Social Media Flow");
                
            }, "Social media presence verification with detailed analysis");
        }

        [Test]
        [Order(5)]
        [Category("Experimental")]
        [Priority(TestPriority.Optional)]
        [Description("Optional test - experimental feature or edge case")]
        public async Task OptionalTest_PerformanceCheck()
        {
            // INFO level - experimental test start
            LogWithLevel("?? OPTIONAL TEST: Performance baseline measurement", LogLevel.Info, "Experimental");
            
            await ExecuteStepAsync("Basic performance check", async () =>
            {
                // TRACE level - performance measurement setup
                LogWithLevel("Initializing performance measurement infrastructure", LogLevel.Trace, "Performance Setup");
                
                // DEBUG level - timing start
                LogWithLevel("Starting high-precision timing measurement", LogLevel.Debug, "Timing");
                var startTime = DateTime.Now;
                
                // INFO level - performance test execution
                LogWithLevel("Executing page load for performance measurement", LogLevel.Info, "Performance Test");
                await Page.GotoAsync("https://playwright.dev");
                
                // DEBUG level - timing calculation
                var loadTime = DateTime.Now - startTime;
                var loadTimeMs = loadTime.TotalMilliseconds;
                LogWithLevel($"Calculated page load time: {loadTimeMs:F2}ms", LogLevel.Debug, "Timing Calculation");
                
                // INFO level - performance result
                LogWithLevel($"Performance measurement complete: {loadTimeMs:F2}ms load time", LogLevel.Info, "Performance Result");
                Logger.LogTestData("Page load time", $"{loadTimeMs:F2}ms");
                
                // Performance threshold analysis with different log levels
                if (loadTime.TotalSeconds < 2)
                {
                    // INFO level - excellent performance
                    LogWithLevel("? Excellent performance: Load time under 2 seconds", LogLevel.Info, "Performance Grade");
                }
                else if (loadTime.TotalSeconds < 5)
                {
                    // WARNING level - acceptable performance
                    LogWithLevel("?? Acceptable performance: Load time under 5 seconds", LogLevel.Warning, "Performance Grade");
                }
                else
                {
                    // ERROR level - poor performance (but acceptable for optional test)
                    LogWithLevel("? Poor performance detected, but acceptable for optional test", LogLevel.Error, "Performance Issue");
                    
                    // DEBUG level - investigation suggestion
                    LogWithLevel("Consider investigating network conditions or page optimization", LogLevel.Debug, "Performance Advice");
                }
                
                // TRACE level - measurement completion
                LogWithLevel("Performance measurement infrastructure cleanup completed", LogLevel.Trace, "Performance Setup");
                
            }, "Optional performance baseline measurement with detailed analysis");
        }

        [Test]
        [Order(6)]
        [Category("LogLevelDemo")]
        [Priority(TestPriority.Medium)]
        [Description("Demonstration of all ReportPortal log levels for filtering")]
        public async Task LogLevelFilteringDemo()
        {
            // Demonstrate comprehensive log level usage for ReportPortal filtering
            LogWithLevel("?? LOG LEVEL FILTERING DEMONSTRATION", LogLevel.Info, "Demo Start");
            
            await ExecuteStepAsync("Demonstrate all log levels", async () =>
            {
                // Show all available log levels with clear examples
                LogAllLevels("Log Level Demonstration", "Filtering Demo");
                
                // Simulate different scenarios with appropriate log levels
                LogWithLevel("Simulating trace-level debugging information", LogLevel.Trace, "Trace Example");
                LogWithLevel("Simulating debug-level technical details", LogLevel.Debug, "Debug Example");
                LogWithLevel("Simulating standard informational message", LogLevel.Info, "Info Example");
                LogWithLevel("Simulating warning for attention-needed situations", LogLevel.Warning, "Warning Example");
                LogWithLevel("Simulating error for failure conditions", LogLevel.Error, "Error Example");
                LogWithLevel("Simulating fatal error for critical system failures", LogLevel.Fatal, "Fatal Example");
                
                // Demonstrate filtering scenarios
                LogFilteringScenarios();
                
                await Task.Delay(100); // Simulate some work
                
            }, "Complete demonstration of ReportPortal log level filtering capabilities");
        }

        /// <summary>
        /// Log a message with a specific log level for ReportPortal filtering
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logLevel">The ReportPortal log level</param>
        /// <param name="category">Optional category for the log</param>
        private void LogWithLevel(string message, LogLevel logLevel, string category = "")
        {
            try
            {
                // Use the ReportPortal logger with specific log level
                Logger.ReportPortal.LogCustom(message, GetColorForLogLevel(logLevel), true, logLevel);
                
                // Also log to console with level indication
                var levelText = logLevel.ToString().ToUpper();
                var categoryText = !string.IsNullOrEmpty(category) ? $"[{category}] " : "";
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{levelText}] {categoryText}{message}");
                TestContext.WriteLine($"[{levelText}] {categoryText}{message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log with level {logLevel}: {ex.Message}");
                TestContext.WriteLine($"Failed to log with level {logLevel}: {ex.Message}");
            }
        }

        /// <summary>
        /// Demonstrate all available log levels for ReportPortal filtering
        /// </summary>
        /// <param name="context">Context description</param>
        /// <param name="category">Log category</param>
        private void LogAllLevels(string context, string category)
        {
            LogWithLevel($"?? TRACE: {context} - Detailed execution flow tracking", LogLevel.Trace, category);
            LogWithLevel($"?? DEBUG: {context} - Technical debugging information", LogLevel.Debug, category);
            LogWithLevel($"?? INFO: {context} - General informational messages", LogLevel.Info, category);
            LogWithLevel($"?? WARNING: {context} - Important alerts and notifications", LogLevel.Warning, category);
            LogWithLevel($"? ERROR: {context} - Error conditions and failures", LogLevel.Error, category);
            LogWithLevel($"?? FATAL: {context} - Critical system failures", LogLevel.Fatal, category);
        }

        /// <summary>
        /// Demonstrate different filtering scenarios for ReportPortal
        /// </summary>
        private void LogFilteringScenarios()
        {
            // Scenario 1: Development/Debugging (show TRACE and DEBUG)
            LogWithLevel("SCENARIO 1: Development debugging - use TRACE & DEBUG levels", LogLevel.Debug, "Filtering Guide");
            
            // Scenario 2: Production monitoring (show INFO, WARNING, ERROR, FATAL)
            LogWithLevel("SCENARIO 2: Production monitoring - use INFO, WARN, ERROR, FATAL levels", LogLevel.Info, "Filtering Guide");
            
            // Scenario 3: Error investigation (show ERROR and FATAL only)
            LogWithLevel("SCENARIO 3: Error investigation - use ERROR & FATAL levels only", LogLevel.Error, "Filtering Guide");
            
            // Scenario 4: System health monitoring (show WARNING, ERROR, FATAL)
            LogWithLevel("SCENARIO 4: Health monitoring - use WARN, ERROR, FATAL levels", LogLevel.Warning, "Filtering Guide");
        }

        /// <summary>
        /// Get appropriate color for each log level
        /// </summary>
        /// <param name="logLevel">The log level</param>
        /// <returns>Color name for formatting</returns>
        private string GetColorForLogLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => "gray",
            LogLevel.Debug => "blue",
            LogLevel.Info => "green",
            LogLevel.Warning => "orange",
            LogLevel.Error => "red",
            LogLevel.Fatal => "darkred",
            _ => "black"
        };

        /// <summary>
        /// Enhanced logging method that outputs to both ReportPortal and Console
        /// </summary>
        private void LogAndConsole(string message, string category = "")
        {
            // Log to ReportPortal
            try
            {
                Logger.LogInfo(message, category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReportPortal logging failed: {ex.Message}");
            }
            
            // Always log to console as fallback
            var categoryText = !string.IsNullOrEmpty(category) ? $"[{category}] " : "";
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} {categoryText}{message}");
            
            // Also log to NUnit test context
            TestContext.WriteLine($"{categoryText}{message}");
        }

        /// <summary>
        /// Log priority information with enhanced visibility
        /// </summary>
        private void LogPriorityAndConsole(string message, string category = "")
        {
            LogAndConsole(message, category);
            
            // Add extra visibility for priority messages
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"PRIORITY MESSAGE: {message}");
            Console.WriteLine(new string('=', 80));
        }
    }
}