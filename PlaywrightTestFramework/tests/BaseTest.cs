using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTestFramework.Reporting;
using System.Reflection;
using PlaywrightTestFramework.tests;

namespace PlaywrightTestFramework.Tests
{
    /// <summary>
    /// Base test class with ReportPortal integration
    /// </summary>
    public abstract class BaseTest : PageTest
    {
        [SetUp]
        public async Task SetUp()
        {
            ReportPortalHelper.InitializeTest();
            
            // Get test method information
            var testMethod = GetType().GetMethod(TestContext.CurrentContext.Test.MethodName!);
            var testName = TestContext.CurrentContext.Test.Name;
            
            // Log basic test information with enhanced HTML formatting
            ReportPortalHelper.LogInfo($"<div style='background-color: #E6F3FF; border-left: 4px solid #007ACC; padding: 10px; margin: 5px 0;'>" +
                                     $"<span style='color: #007ACC; font-weight: bold; font-size: 16px;'>&#x1F9EA;</span> " +
                                     $"<strong>Test '{testName}' Started</strong>" +
                                     $"</div>");
            
            // Log test priority information
            LogTestPriorityInfo(testMethod);
            
            // Log test categories
            LogTestCategories();
            
            // Log test description if available
            LogTestDescription(testMethod);
        }

        private void LogTestPriorityInfo(MethodInfo? testMethod)
        {
            if (testMethod != null)
            {
                var priorityAttribute = testMethod.GetCustomAttribute<PriorityAttribute>();
                if (priorityAttribute != null)
                {
                    var priorityIcon = GetPriorityIcon(priorityAttribute.Priority);
                    var priorityHtmlIcon = GetPriorityHtmlIcon(priorityAttribute.Priority);
                    
                    ReportPortalHelper.LogInfo($"{priorityHtmlIcon} {priorityIcon} Test Priority: {priorityAttribute.Priority.ToString().ToUpper()}");
                    
                    // Add priority-specific guidance
                    var priorityGuidance = GetPriorityGuidance(priorityAttribute.Priority);
                    if (!string.IsNullOrEmpty(priorityGuidance))
                    {
                        ReportPortalHelper.LogInfo($"<span style='color: #4682B4; font-weight: bold;'>&#x1F4CB;</span> {priorityGuidance}");
                    }
                }
            }
        }

        private void LogTestCategories()
        {
            var categories = TestContext.CurrentContext.Test.Properties["Category"];
            if (categories != null && categories.Any())
            {
                var categoryList = string.Join(", ", categories.Cast<string>());
                ReportPortalHelper.LogInfo($"<span style='color: #9932CC; font-weight: bold;'>&#x1F3F7;</span> Categories: <span style='background-color: #F0E6FF; padding: 2px 6px; border-radius: 3px; color: #9932CC; font-weight: bold;'>{categoryList}</span>");
            }
        }

        private void LogTestDescription(MethodInfo? testMethod)
        {
            if (testMethod != null)
            {
                var descriptionAttribute = testMethod.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    // Try to get the description text through reflection since it might be a different property
                    var descriptionText = TestContext.CurrentContext.Test.Properties.Get("Description") as string;
                    if (!string.IsNullOrEmpty(descriptionText))
                    {
                        ReportPortalHelper.LogInfo($"<span style='color: #2E8B57; font-weight: bold;'>&#x1F4DD;</span> Description: <em style='color: #2E8B57;'>{descriptionText}</em>");
                    }
                }
            }
        }

        private static string GetPriorityIcon(TestPriority priority) => priority switch
        {
            TestPriority.Critical => "<span style='color: #FF0000; font-weight: bold; background-color: #FFE6E6; padding: 2px 6px; border-radius: 3px;'>CRITICAL</span>",
            TestPriority.High => "<span style='color: #FF8C00; font-weight: bold; background-color: #FFF0E6; padding: 2px 6px; border-radius: 3px;'>HIGH</span>", 
            TestPriority.Medium => "<span style='color: #DAA520; font-weight: bold; background-color: #FFFACD; padding: 2px 6px; border-radius: 3px;'>MEDIUM</span>",
            TestPriority.Low => "<span style='color: #32CD32; font-weight: bold; background-color: #F0FFF0; padding: 2px 6px; border-radius: 3px;'>LOW</span>",
            TestPriority.Optional => "<span style='color: #4169E1; font-weight: bold; background-color: #E6F0FF; padding: 2px 6px; border-radius: 3px;'>OPTIONAL</span>",
            _ => "<span style='color: #808080; font-weight: bold; background-color: #F0F0F0; padding: 2px 6px; border-radius: 3px;'>UNKNOWN</span>"
        };

        private static string GetPriorityHtmlIcon(TestPriority priority) => priority switch
        {
            TestPriority.Critical => "&#x1F534;", // Red circle
            TestPriority.High => "&#x1F7E0;",     // Orange circle
            TestPriority.Medium => "&#x1F7E1;",   // Yellow circle
            TestPriority.Low => "&#x1F7E2;",      // Green circle
            TestPriority.Optional => "&#x1F535;", // Blue circle
            _ => "&#x26AA;"                        // White circle
        };

        private static string GetPriorityColor(TestPriority priority) => priority switch
        {
            TestPriority.Critical => "red",
            TestPriority.High => "orange",
            TestPriority.Medium => "yellow", 
            TestPriority.Low => "green",
            TestPriority.Optional => "blue",
            _ => "gray"
        };

        private static string GetPriorityGuidance(TestPriority priority) => priority switch
        {
            TestPriority.Critical => "CRITICAL: Must pass - blocks release if failed",
            TestPriority.High => "HIGH: Core functionality - investigate failures immediately",
            TestPriority.Medium => "MEDIUM: Important feature - review failures promptly",
            TestPriority.Low => "LOW: Nice-to-have feature - review when time permits",
            TestPriority.Optional => "OPTIONAL: Edge case or experimental - review if needed",
            _ => ""
        };

        static void Main()
        {
        }

        [TearDown]
        public async Task TearDown()
        {
            var result = TestContext.CurrentContext.Result;
            var testName = TestContext.CurrentContext.Test.Name;
            
            // Take screenshot on failure
            if (result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var screenshotPath = $"screenshot_{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                try
                {
                    Page.ScreenshotAsync(new() { Path = screenshotPath }).Wait();
                    ReportPortalHelper.AddScreenshot(screenshotPath, "Test Failed - Screenshot");
                }
                catch (Exception ex)
                {
                    ReportPortalHelper.LogWarn($"Failed to take screenshot: {ex.Message}");
                }
                
                // Log failure with priority context
                var testMethod = GetType().GetMethod(TestContext.CurrentContext.Test.MethodName!);
                var priorityAttribute = testMethod?.GetCustomAttribute<PriorityAttribute>();
                var priorityIcon = priorityAttribute != null ? GetPriorityIcon(priorityAttribute.Priority) : "<span style='color: #FF0000; font-weight: bold;'>FAILED</span>";
                
                ReportPortalHelper.LogError($"{priorityIcon} Test '{testName}' FAILED: {result.Message}");
                
                if (priorityAttribute?.Priority == TestPriority.Critical)
                {
                    ReportPortalHelper.LogError("<span style='color: #FF0000; font-weight: bold; background-color: #FFE6E6; padding: 5px 10px; border: 2px solid #FF0000; border-radius: 5px;'>?? CRITICAL TEST FAILURE - IMMEDIATE ATTENTION REQUIRED!</span>");
                }
            }
            else if (result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                var testMethod = GetType().GetMethod(TestContext.CurrentContext.Test.MethodName!);
                var priorityAttribute = testMethod?.GetCustomAttribute<PriorityAttribute>();
                var priorityIcon = priorityAttribute != null ? GetPriorityIcon(priorityAttribute.Priority) : "<span style='color: #32CD32; font-weight: bold;'>PASSED</span>";
                
                ReportPortalHelper.LogInfo($"{priorityIcon} Test '{testName}' PASSED successfully");
            }
            else if (result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Skipped)
            {
                ReportPortalHelper.LogWarn($"<span style='color: #FFA500; font-weight: bold;'>SKIPPED</span> Test '{testName}' was SKIPPED: {result.Message}");
            }

            ReportPortalHelper.Cleanup();
        }

        /// <summary>
        /// Execute a test step with automatic ReportPortal logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="stepAction">Action to execute</param>
        /// <param name="description">Optional description</param>
        protected async Task ExecuteStepAsync(string stepName, Func<Task> stepAction, string description = "")
        {
            var stepId = ReportPortalHelper.StartTestStep(stepName, description);
            
            try
            {
                await stepAction();
                ReportPortalHelper.EndTestStep(stepId, ReportPortal.Client.Abstractions.Models.Status.Passed, $"Step '{stepName}' completed successfully");
            }
            catch (Exception ex)
            {
                ReportPortalHelper.EndTestStep(stepId, ReportPortal.Client.Abstractions.Models.Status.Failed, $"Step '{stepName}' failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Execute a test step with automatic ReportPortal logging (synchronous version)
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="stepAction">Action to execute</param>
        /// <param name="description">Optional description</param>
        protected void ExecuteStep(string stepName, Action stepAction, string description = "")
        {
            var stepId = ReportPortalHelper.StartTestStep(stepName, description);
            
            try
            {
                stepAction();
                ReportPortalHelper.EndTestStep(stepId, ReportPortal.Client.Abstractions.Models.Status.Passed, $"Step '{stepName}' completed successfully");
            }
            catch (Exception ex)
            {
                ReportPortalHelper.EndTestStep(stepId, ReportPortal.Client.Abstractions.Models.Status.Failed, $"Step '{stepName}' failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Log an info message to ReportPortal
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogInfo(string message) => ReportPortalHelper.LogInfo(message);

        /// <summary>
        /// Log a warning message to ReportPortal
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogWarn(string message) => ReportPortalHelper.LogWarn(message);

        /// <summary>
        /// Log an error message to ReportPortal
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogError(string message) => ReportPortalHelper.LogError(message);

        /// <summary>
        /// Log a debug message to ReportPortal
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void LogDebug(string message) => ReportPortalHelper.LogDebug(message);
    }
}