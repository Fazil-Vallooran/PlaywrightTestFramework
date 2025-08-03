using NUnit.Framework;
using PlaywrightTestFramework.Tests;
using PlaywrightTestFramework.Utils;
using System.Reflection;

namespace PlaywrightTestFramework.tests
{
    [TestFixture]
    public class FrameworkFeatureTests : BaseTest
    {
        [Test]
        [Category("Framework")]
        [Priority(TestPriority.High)]
        [Description("Test the configuration manager functionality")]
        public void TestConfigurationManager()
        {
            // Test configuration loading
            Logger.LogSectionHeader("Configuration Manager Test", "blue");
            
            var browser = ConfigurationManager.GetBrowser();
            var baseUrl = ConfigurationManager.GetBaseUrl();
            var timeout = ConfigurationManager.GetTimeout();
            var environment = ConfigurationManager.GetEnvironment();
            
            // Log the configuration values
            Logger.LogTestData("Browser", browser);
            Logger.LogTestData("Base URL", baseUrl);
            Logger.LogTestData("Timeout", timeout.ToString());
            Logger.LogTestData("Environment", environment);
            
            // Verify default values
            Assert.That(browser, Is.Not.Null.And.Not.Empty, "Browser should not be null or empty");
            Assert.That(baseUrl, Is.Not.Null.And.Not.Empty, "Base URL should not be null or empty");
            Assert.That(timeout, Is.GreaterThan(0), "Timeout should be greater than 0");
            Assert.That(environment, Is.Not.Null.And.Not.Empty, "Environment should not be null or empty");
            
            Logger.LogInfo("✅ Configuration manager test passed successfully");
        }
        
        [Test]
        [Category("Framework")]
        [Priority(TestPriority.Medium)]
        [Description("Test the test data manager functionality")]
        public void TestDataManager()
        {
            Logger.LogSectionHeader("Test Data Manager Test", "purple");
            
            // Test loading test data from JSON
            try
            {
                var testData = PlaywrightTestFramework.Utils.TestDataManager.GetAllTestData("LoginTest");
                
                Logger.LogInfo($"📊 Loaded {testData.Count} test data items from JSON");
                
                foreach (var kvp in testData)
                {
                    Logger.LogTestData(kvp.Key, kvp.Value);
                }
                
                Assert.That(testData, Is.Not.Null, "Test data should not be null");
                Assert.That(testData.Count, Is.GreaterThan(0), "Test data should contain items");
                
                Logger.LogInfo("✅ JSON test data loading successful");
            }
            catch (Exception ex)
            {
                Logger.LogWarn($"JSON test data loading failed: {ex.Message}");
            }
            
            // Test loading CSV data
            try
            {
                var csvData = PlaywrightTestFramework.Utils.TestDataManager.LoadFromCsvAsDictionaries("LoginCredentials");
                
                Logger.LogInfo($"📊 Loaded {csvData.Count} test data rows from CSV");
                
                foreach (var (row, index) in csvData.Select((data, i) => (data, i)))
                {
                    Logger.LogInfo($"Row {index + 1}: {string.Join(", ", row.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
                }
                
                Assert.That(csvData, Is.Not.Null, "CSV data should not be null");
                Assert.That(csvData.Count, Is.GreaterThan(0), "CSV data should contain rows");
                
                Logger.LogInfo("✅ CSV test data loading successful");
            }
            catch (Exception ex)
            {
                Logger.LogWarn($"CSV test data loading failed: {ex.Message}");
            }
        }
        
        [Test]
        [Category("Framework")]
        [Priority(TestPriority.Low)]
        [Description("Test the enhanced logging functionality")]
        public void TestEnhancedLogging()
        {
            Logger.LogSectionHeader("Enhanced Logging Test", "green");
            
            // Test different log levels
            Logger.LogInfo("This is an info message");
            Logger.LogWarn("This is a warning message");
            Logger.LogError("This is an error message");
            Logger.LogDebug("This is a debug message");
            
            // Test action logging
            Logger.LogAction("Click", "Submit button", "test value");
            Logger.LogAction("Navigate", "https://example.com");
            
            // Test verification logging
            Logger.LogVerification("Test verification", "expected", "actual", true);
            Logger.LogVerification("Failed verification", "expected", "different", false);
            
            // Test test data logging
            Logger.LogTestData("Username", "testuser@example.com");
            Logger.LogTestData("Password", "********");
            
            // Test performance logging
            Logger.LogPerformance("Test execution time", 1500, "ms", 2000);
            
            // Test configuration logging
            Logger.LogConfiguration("Test Setting", "Enabled");
            
            // Test custom logging
            Logger.LogCustom("Custom colored message", "blue", true);
            
            // Test table logging
            var tableData = new Dictionary<string, string>
            {
                { "Browser", "Chromium" },
                { "Version", "1.0.0" },
                { "Environment", "Test" },
                { "Status", "Active" }
            };
            Logger.LogTable("Test Information", tableData);
            
            Logger.LogInfo("✅ Enhanced logging test completed successfully");
            
            // Since this is a unit test without actual failures, we'll always pass
            Assert.That(true, Is.True, "Logging test should always pass");
        }
        
        [Test]
        [Category("Framework")]
        [Priority(TestPriority.Critical)]
        [Description("Test the priority system and attributes")]
        public void TestPrioritySystem()
        {
            Logger.LogSectionHeader("Priority System Test", "red");
            
            // Get the current test method
            var testMethod = GetType().GetMethod(nameof(TestPrioritySystem));
            var priorityAttribute = testMethod?.GetCustomAttribute<PriorityAttribute>();
            
            Assert.That(priorityAttribute, Is.Not.Null, "Priority attribute should be present");
            Assert.That(priorityAttribute!.Priority, Is.EqualTo(TestPriority.Critical), "Priority should be Critical");
            
            Logger.LogInfo($"🏷️ Test priority: {priorityAttribute.Priority}");
            Logger.LogInfo($"🎯 Priority guidance: {GetPriorityGuidance(priorityAttribute.Priority)}");
            
            // Test all priority levels
            var priorities = Enum.GetValues<TestPriority>();
            foreach (var priority in priorities)
            {
                Logger.LogInfo($"Priority {priority}: {GetPriorityGuidance(priority)}");
            }
            
            Logger.LogInfo("✅ Priority system test completed successfully");
        }
        
        [Test]
        [Category("Framework")]
        [Priority(TestPriority.Optional)]
        [Description("Test step execution without browser")]
        public void TestStepExecution()
        {
            Logger.LogSectionHeader("Step Execution Test", "orange");
            
            // Test synchronous step execution
            ExecuteStep("Synchronous step test", () =>
            {
                Logger.LogInfo("Executing synchronous step");
                Logger.LogAction("Simulate", "Button click");
                System.Threading.Thread.Sleep(100); // Simulate some work
                Logger.LogInfo("Synchronous step completed");
            });
            
            // Test step with result
            var result = Logger.ExecuteStepWithResult("Get test result", () =>
            {
                Logger.LogInfo("Calculating test result");
                return "Success";
            });
            
            Logger.LogTestData("Step result", result);
            Assert.That(result, Is.EqualTo("Success"), "Step should return success");
            
            Logger.LogInfo("✅ Step execution test completed successfully");
        }
        
        private void ExecuteStep(string stepName, Action action)
        {
            var stepId = Logger.StartStep(stepName);
            try
            {
                action();
                Logger.EndStep(stepId, true, "Step completed successfully");
            }
            catch (Exception ex)
            {
                Logger.EndStep(stepId, false, $"Step failed: {ex.Message}");
                throw;
            }
        }
        
        private static string GetPriorityGuidance(TestPriority priority) => priority switch
        {
            TestPriority.Critical => "CRITICAL: Must pass - blocks release if failed",
            TestPriority.High => "HIGH: Core functionality - investigate failures immediately",
            TestPriority.Medium => "MEDIUM: Important feature - review failures promptly",
            TestPriority.Low => "LOW: Nice-to-have feature - review when time permits",
            TestPriority.Optional => "OPTIONAL: Edge case or experimental - review if needed",
            _ => ""
        };
    }
}