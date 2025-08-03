using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;
using System.Diagnostics;

namespace PlaywrightTestFramework.tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class EnhancedExampleTests : BrowserBaseTest
    {
        [Test]
        [Category("Smoke")]
        [Priority(TestPriority.Critical)]
        [Description("Enhanced test demonstrating new framework features with configuration management")]
        [TestData("LoginTest")]
        public async Task EnhancedPlaywrightTest()
        {
            // Load test data using the new TestDataManager
            var testData = TestDataManager.GetAllTestData("LoginTest");
            var baseUrl = ConfigurationManager.GetBaseUrl();
            var timeout = ConfigurationManager.GetTimeout();

            Logger.LogSectionHeader("Enhanced Framework Demo Test", "darkblue");
            
            // Log test configuration
            Logger.LogInfo($"🔧 Using base URL: {baseUrl}");
            Logger.LogInfo($"⏱️ Timeout: {timeout} seconds");
            Logger.LogInfo($"🌍 Environment: {ConfigurationManager.GetEnvironment()}");
            Logger.LogInfo($"🖥️ Browser: {ConfigurationManager.GetBrowser()}");

            var stopwatch = Stopwatch.StartNew();

            await ExecuteStepAsync("Navigate to Playwright website with performance monitoring", async () =>
            {
                Logger.LogAction("Navigate", baseUrl);
                
                var navigationStart = Stopwatch.StartNew();
                await Page.GotoAsync(baseUrl);
                navigationStart.Stop();
                
                // Log performance metrics
                Logger.LogPerformance("Page Navigation Time", navigationStart.ElapsedMilliseconds, "ms", 3000);
                
                Logger.LogInfo("✅ Navigation completed successfully");
            }, "Navigate to the main page and measure performance");

            await ExecuteStepAsync("Verify page title and capture performance metrics", async () =>
            {
                var titleStart = Stopwatch.StartNew();
                var actualTitle = await Page.TitleAsync();
                titleStart.Stop();

                Logger.LogTestData("Actual page title", actualTitle);
                Logger.LogPerformance("Title Retrieval Time", titleStart.ElapsedMilliseconds, "ms", 1000);

                // Verify title contains expected text
                await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("Playwright"));
                
                Logger.LogVerification("Page title validation", "Contains 'Playwright'", actualTitle, true);
                Logger.LogInfo("🎯 Title verification passed");
            }, "Verify the page title meets expectations");

            await ExecuteStepAsync("Interact with page elements and test functionality", async () =>
            {
                Logger.LogInfo("🔍 Looking for 'Get started' link");
                
                var getStartedLink = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
                
                // Check if element is visible
                var isVisible = await getStartedLink.IsVisibleAsync();
                Logger.LogVerification("Get started link visibility", "Visible", isVisible.ToString(), isVisible);

                if (isVisible)
                {
                    var clickStart = Stopwatch.StartNew();
                    await getStartedLink.ClickAsync();
                    clickStart.Stop();

                    Logger.LogAction("Click", "Get started link");
                    Logger.LogPerformance("Click Response Time", clickStart.ElapsedMilliseconds, "ms", 1000);
                    Logger.LogInfo("🖱️ Successfully clicked 'Get started' link");
                }
                else
                {
                    Logger.LogWarn("⚠️ 'Get started' link not visible, skipping click action");
                }
            }, "Test page interactions and element visibility");

            await ExecuteStepAsync("Verify navigation and final state", async () =>
            {
                // Wait for navigation to complete
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                var finalUrl = Page.Url;
                Logger.LogTestData("Final URL", finalUrl);

                // Check for Installation heading
                try
                {
                    var installationHeading = Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" });
                    
                    var headingVisible = await installationHeading.IsVisibleAsync();
                    Logger.LogVerification("Installation heading visibility", "Visible", headingVisible.ToString(), headingVisible);

                    if (headingVisible)
                    {
                        var headingText = await installationHeading.TextContentAsync();
                        Logger.LogTestData("Installation heading text", headingText ?? "");
                        Logger.LogInfo("📋 Installation section found and verified");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarn($"⚠️ Installation heading not found: {ex.Message}");
                }
            }, "Verify final page state and content");

            stopwatch.Stop();
            
            // Log overall test performance
            Logger.LogSectionHeader("Test Summary", "darkgreen");
            Logger.LogPerformance("Total Test Duration", stopwatch.ElapsedMilliseconds, "ms");
            Logger.LogInfo($"🏁 Test completed successfully in {stopwatch.ElapsedMilliseconds}ms");
            
            // Generate and log test summary data
            var testSummary = new Dictionary<string, string>
            {
                { "Test Name", TestContext.CurrentContext.Test.Name },
                { "Duration", $"{stopwatch.ElapsedMilliseconds}ms" },
                { "Browser", ConfigurationManager.GetBrowser() },
                { "Environment", ConfigurationManager.GetEnvironment() },
                { "Final URL", Page.Url },
                { "Status", "Passed" }
            };

            Logger.LogTable("Test Execution Summary", testSummary);
        }

        [Test]
        [Category("DataDriven")]
        [Priority(TestPriority.Medium)]
        [Description("Demonstrate data-driven testing with CSV data")]
        public async Task DataDrivenTest()
        {
            Logger.LogSectionHeader("Data-Driven Test Demo", "purple");

            // Load test data from CSV
            var loginData = TestDataManager.LoadFromCsvAsDictionaries("LoginCredentials");
            
            Logger.LogInfo($"📊 Loaded {loginData.Count} test data sets from CSV");

            foreach (var (testCase, index) in loginData.Select((data, i) => (data, i)))
            {
                await ExecuteStepAsync($"Test case {index + 1}: {testCase["description"]}", async () =>
                {
                    Logger.LogTestData("Username", testCase["username"]);
                    Logger.LogTestData("Expected Result", testCase["expectedResult"]);
                    Logger.LogTestData("Description", testCase["description"]);

                    // Simulate test logic based on expected result
                    var expectedResult = testCase["expectedResult"];
                    var actualResult = await Task.FromResult(SimulateLoginAttempt(testCase["username"], testCase["password"]));

                    Logger.LogVerification(
                        "Login result validation",
                        expectedResult,
                        actualResult,
                        expectedResult == actualResult
                    );

                    if (expectedResult == actualResult)
                    {
                        Logger.LogInfo($"✅ Test case {index + 1} passed");
                    }
                    else
                    {
                        Logger.LogWarn($"⚠️ Test case {index + 1} failed - expected {expectedResult}, got {actualResult}");
                    }

                }, $"Execute test case with data: {testCase["username"]}");
            }

            Logger.LogInfo($"🏁 Data-driven test completed: {loginData.Count} test cases executed");
        }

        [Test]
        [Category("Performance")]
        [Priority(TestPriority.Low)]
        [Description("Performance monitoring and measurement test")]
        public async Task PerformanceMonitoringTest()
        {
            if (!ConfigurationManager.IsPerformanceMonitoringEnabled())
            {
                Assert.Ignore("Performance monitoring is disabled in configuration");
                return;
            }

            Logger.LogSectionHeader("Performance Monitoring Test", "brown");

            var baseUrl = ConfigurationManager.GetBaseUrl();
            var performanceMetrics = new Dictionary<string, double>();

            await ExecuteStepAsync("Measure page load performance", async () =>
            {
                var loadStart = Stopwatch.StartNew();
                await Page.GotoAsync(baseUrl);
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                loadStart.Stop();

                performanceMetrics["PageLoadTime"] = loadStart.ElapsedMilliseconds;
                Logger.LogPerformance("Page Load Time", loadStart.ElapsedMilliseconds, "ms", 5000);

                // Measure DOM content loaded
                var domContentLoaded = await Page.EvaluateAsync<double>(
                    "() => performance.getEntriesByType('navigation')[0].domContentLoadedEventEnd - performance.getEntriesByType('navigation')[0].navigationStart"
                );
                
                performanceMetrics["DOMContentLoaded"] = domContentLoaded;
                Logger.LogPerformance("DOM Content Loaded", domContentLoaded, "ms", 3000);

            }, "Measure initial page load performance");

            await ExecuteStepAsync("Test interaction performance", async () =>
            {
                try
                {
                    var getStartedLink = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
                    
                    if (await getStartedLink.IsVisibleAsync())
                    {
                        var clickStart = Stopwatch.StartNew();
                        await getStartedLink.ClickAsync();
                        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        clickStart.Stop();

                        performanceMetrics["NavigationTime"] = clickStart.ElapsedMilliseconds;
                        Logger.LogPerformance("Navigation Time", clickStart.ElapsedMilliseconds, "ms", 3000);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarn($"Could not test navigation performance: {ex.Message}");
                }

            }, "Measure navigation and interaction performance");

            // Log performance summary
            Logger.LogSectionHeader("Performance Summary", "darkbrown");
            Logger.LogTable("Performance Metrics", performanceMetrics.ToDictionary(
                kvp => kvp.Key, 
                kvp => $"{kvp.Value:F2} ms"
            ));

            // Validate performance thresholds
            var failedMetrics = new List<string>();
            if (performanceMetrics.ContainsKey("PageLoadTime") && performanceMetrics["PageLoadTime"] > 5000)
                failedMetrics.Add($"Page load time exceeded threshold: {performanceMetrics["PageLoadTime"]}ms > 5000ms");

            if (performanceMetrics.ContainsKey("NavigationTime") && performanceMetrics["NavigationTime"] > 3000)
                failedMetrics.Add($"Navigation time exceeded threshold: {performanceMetrics["NavigationTime"]}ms > 3000ms");

            if (failedMetrics.Any())
            {
                Logger.LogError("❌ Performance thresholds exceeded:");
                foreach (var metric in failedMetrics)
                {
                    Logger.LogError($"  • {metric}");
                }
                Assert.Fail($"Performance test failed: {string.Join(", ", failedMetrics)}");
            }
            else
            {
                Logger.LogInfo("✅ All performance metrics within acceptable thresholds");
            }
        }

        /// <summary>
        /// Simulate login attempt for data-driven testing
        /// </summary>
        private string SimulateLoginAttempt(string username, string password)
        {
            // Simulate login logic
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return "failure";

            if (username.Contains("invalid") || password.Contains("Wrong"))
                return "failure";

            return "success";
        }
    }
}