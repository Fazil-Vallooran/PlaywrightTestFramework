using ReportPortal.Shared;
using ReportPortal.Shared.Extensibility;
using ReportPortal.Shared.Reporter;
using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using PlaywrightTestFramework.Extra;
using ReportPortal.Client;
using NUnit.Framework;

namespace PlaywrightTestFramework.Reporting
{
    public static class ReportPortalHelper
    {
        private static ITestReporter? _currentTestReporter;
        private static readonly Dictionary<string, ITestReporter> _activeSteps = new();

        /// <summary>
        /// Get the current test reporter instance
        /// </summary>
        /// <returns>Current test reporter or null if not initialized</returns>
        public static ITestReporter? GetCurrentTestReporter()
        {
            return _currentTestReporter;
        }

        /// <summary>
        /// Initialize ReportPortal for the current test
        /// </summary>
        public static void InitializeTest()
        {
            try
            {
                // Use the NUnit attributes to provide context for ReportPortal
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal test context initialized");
                TestContext.WriteLine("ReportPortal test context initialized");
                _currentTestReporter = null; // Let ReportPortal handle the reporter internally
            }
            catch (Exception ex)
            {
                // If ReportPortal is not properly initialized, we'll just continue without it
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal initialization failed: {ex.Message}");
                TestContext.WriteLine($"ReportPortal initialization failed: {ex.Message}");
                _currentTestReporter = null;
            }
        }

        /// <summary>
        /// Start a test step
        /// </summary>
        /// <param name="stepName">Name of the test step</param>
        /// <param name="description">Description of the test step</param>
        /// <returns>Step ID for tracking</returns>
        public static string StartTestStep(string stepName, string description = "")
        {
            var stepId = Guid.NewGuid().ToString();
            
            try
            {
                // Enhanced HTML formatting for step start
                var stepStartMessage = $"<div style='background-color: #E6F7FF; border-left: 4px solid #1890FF; padding: 8px; margin: 3px 0;'>" +
                                     $"<span style='color: #1890FF; font-weight: bold;'>&#x1F680;</span> " +
                                     $"<strong style='color: #1890FF;'>STEP STARTED:</strong> {stepName}" +
                                     $"</div>";
                
                LogInfo(stepStartMessage);
                
                if (!string.IsNullOrEmpty(description))
                {
                    var descriptionMessage = $"<div style='background-color: #F6FFED; border-left: 4px solid #52C41A; padding: 6px; margin: 2px 0 2px 20px;'>" +
                                           $"<span style='color: #52C41A; font-weight: bold;'>&#x1F4DD;</span> " +
                                           $"<em style='color: #52C41A;'>Description: {description}</em>" +
                                           $"</div>";
                    LogInfo(descriptionMessage);
                }
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ===== STEP STARTED: {stepName} =====");
                if (!string.IsNullOrEmpty(description))
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Description: {description}");
                }
                TestContext.WriteLine($"STEP STARTED: {stepName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error starting test step '{stepName}': {ex.Message}");
                TestContext.WriteLine($"Error starting test step '{stepName}': {ex.Message}");
            }

            return stepId;
        }

        /// <summary>
        /// End a test step
        /// </summary>
        /// <param name="stepId">Step ID returned from StartTestStep</param>
        /// <param name="status">Status of the step</param>
        /// <param name="message">Optional message for the step completion</param>
        public static void EndTestStep(string stepId, Status status = Status.Passed, string message = "")
        {
            try
            {
                var (statusColor, statusIcon, backgroundColor) = status switch
                {
                    Status.Passed => ("#52C41A", "&#x2705;", "#F6FFED"), // Green
                    Status.Failed => ("#FF4D4F", "&#x274C;", "#FFF2F0"), // Red
                    Status.Skipped => ("#FAAD14", "&#x23ED;", "#FFFBE6"), // Yellow
                    _ => ("#1890FF", "&#x2139;", "#E6F7FF") // Blue
                };

                var stepEndMessage = $"<div style='background-color: {backgroundColor}; border-left: 4px solid {statusColor}; padding: 8px; margin: 3px 0;'>" +
                                   $"<span style='color: {statusColor}; font-weight: bold;'>{statusIcon}</span> " +
                                   $"<strong style='color: {statusColor};'>STEP COMPLETED:</strong> Status = {status}" +
                                   $"</div>";

                if (!string.IsNullOrEmpty(message))
                {
                    stepEndMessage += $"<div style='background-color: #FAFAFA; border-left: 4px solid #D9D9D9; padding: 6px; margin: 2px 0 2px 20px;'>" +
                                    $"<span style='color: #595959;'>&#x1F4AC;</span> " +
                                    $"<span style='color: #595959;'>{message}</span>" +
                                    $"</div>";
                }

                switch (status)
                {
                    case Status.Passed:
                        LogInfo(stepEndMessage);
                        break;
                    case Status.Failed:
                        LogError(stepEndMessage);
                        break;
                    case Status.Skipped:
                        LogWarn(stepEndMessage);
                        break;
                }

                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ===== STEP COMPLETED: {status} =====");
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
                }
                TestContext.WriteLine($"STEP COMPLETED: {status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error ending test step: {ex.Message}");
                TestContext.WriteLine($"Error ending test step: {ex.Message}");
            }
        }

        /// <summary>
        /// Log an info message with green color
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogInfo(string message, ITestReporter? reporter = null)
        {
            try
            {
                // Use ReportPortal.Shared.Logging with NUnit attributes
                var coloredMessage = FontColour.FormatInfoMessage(message);
                
                // Add as NUnit test output which ReportPortal should capture
                TestContext.WriteLine($"[ReportPortal-Info] {coloredMessage}");
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [INFO] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [INFO] {message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal logging error: {ex.Message}");
                TestContext.WriteLine($"[INFO] {message}");
            }
        }

        /// <summary>
        /// Log a warning message with yellow color
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogWarn(string message, ITestReporter? reporter = null)
        {
            try
            {
                var coloredMessage = FontColour.FormatWarnMessage(message);
                TestContext.WriteLine($"[ReportPortal-Warning] {coloredMessage}");
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [WARNING] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [WARNING] {message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal logging error: {ex.Message}");
                TestContext.WriteLine($"[WARNING] {message}");
            }
        }

        /// <summary>
        /// Log an error message with red color
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogError(string message, ITestReporter? reporter = null)
        {
            try
            {
                var coloredMessage = FontColour.FormatErrorMessage(message);
                TestContext.WriteLine($"[ReportPortal-Error] {coloredMessage}");
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ERROR] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ERROR] {message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal logging error: {ex.Message}");
                TestContext.WriteLine($"[ERROR] {message}");
            }
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogDebug(string message, ITestReporter? reporter = null)
        {
            try
            {
                TestContext.WriteLine($"[ReportPortal-Debug] {message}");
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [DEBUG] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [DEBUG] {message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal logging error: {ex.Message}");
                TestContext.WriteLine($"[DEBUG] {message}");
            }
        }

        /// <summary>
        /// Log a trace message for detailed debugging
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogTrace(string message, ITestReporter? reporter = null)
        {
            try
            {
                var coloredMessage = $"<span style='color: #808080; font-style: italic;'>[TRACE] {message}</span>";
                TestContext.WriteLine($"[ReportPortal-Trace] {coloredMessage}");
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [TRACE] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [TRACE] {message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal logging error: {ex.Message}");
                TestContext.WriteLine($"[TRACE] {message}");
            }
        }

        /// <summary>
        /// Log a fatal error message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogFatal(string message, ITestReporter? reporter = null)
        {
            try
            {
                var coloredMessage = $"<div style='background-color: #8B0000; color: white; padding: 10px; border: 3px solid #FF0000; font-weight: bold;'>[FATAL] {message}</div>";
                TestContext.WriteLine($"[ReportPortal-Fatal] {coloredMessage}");
                
                // Enhanced console logging
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [FATAL] {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [FATAL] {message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal logging error: {ex.Message}");
                TestContext.WriteLine($"[FATAL] {message}");
            }
        }

        /// <summary>
        /// Log a message with specific log level
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logLevel">The log level to use</param>
        /// <param name="reporter">Optional specific reporter, uses current test reporter if null</param>
        public static void LogWithLevel(string message, LogLevel logLevel, ITestReporter? reporter = null)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    LogTrace(message, reporter);
                    break;
                case LogLevel.Debug:
                    LogDebug(message, reporter);
                    break;
                case LogLevel.Info:
                    LogInfo(message, reporter);
                    break;
                case LogLevel.Warning:
                    LogWarn(message, reporter);
                    break;
                case LogLevel.Error:
                    LogError(message, reporter);
                    break;
                case LogLevel.Fatal:
                    LogFatal(message, reporter);
                    break;
                default:
                    LogInfo(message, reporter);
                    break;
            }
        }

        /// <summary>
        /// Add a screenshot to the current test
        /// </summary>
        /// <param name="screenshotPath">Path to the screenshot file</param>
        /// <param name="message">Optional message to accompany the screenshot</param>
        public static void AddScreenshot(string screenshotPath, string message = "Screenshot")
        {
            try
            {
                if (File.Exists(screenshotPath))
                {
                    TestContext.AddTestAttachment(screenshotPath, message);
                    TestContext.WriteLine($"[ReportPortal-Screenshot] {message}: {screenshotPath}");
                    
                    // Enhanced console logging
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SCREENSHOT] {message}: {screenshotPath}");
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SCREENSHOT] File not found: {screenshotPath}");
                    TestContext.WriteLine($"Screenshot file not found: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SCREENSHOT] Error adding screenshot: {ex.Message}");
                TestContext.WriteLine($"Error adding screenshot: {ex.Message}");
            }
        }

        /// <summary>
        /// Clean up active steps (call this in test cleanup)
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                _activeSteps.Clear();
                _currentTestReporter = null;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ReportPortal cleanup completed");
                TestContext.WriteLine("ReportPortal cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error during cleanup: {ex.Message}");
                TestContext.WriteLine($"Error during cleanup: {ex.Message}");
                _activeSteps.Clear();
                _currentTestReporter = null;
            }
        }
    }
}