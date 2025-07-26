using PlaywrightTestFramework.Extra;
using PlaywrightTestFramework.Reporting;
using ReportPortal.Client.Abstractions.Models;


namespace PlaywrightTestFramework.Utils
{
    /// <summary>
    /// Enhanced generic logger utility for ReportPortal with colored formatting and advanced features
    /// </summary>
    public static class Logger
    {
        private static readonly Lazy<ReportPortalLogger> _reportPortalLogger = new(() => new ReportPortalLogger());
        private static readonly Lazy<TestStepManager> _stepManager = new(() => new TestStepManager());

        /// <summary>
        /// Get the ReportPortal logger instance
        /// </summary>
        public static ReportPortalLogger ReportPortal => _reportPortalLogger.Value;

        /// <summary>
        /// Get the test step manager instance
        /// </summary>
        public static TestStepManager StepManager => _stepManager.Value;

        #region Basic Logging Methods

        /// <summary>
        /// Log an information message with green formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogInfo(string message, string category = "")
        {
            ReportPortal.LogInfo(message, category);
        }

        /// <summary>
        /// Log a warning message with yellow/orange formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogWarn(string message, string category = "")
        {
            ReportPortal.LogWarning(message, category);
        }

        /// <summary>
        /// Log an error message with red formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogError(string message, string category = "")
        {
            ReportPortal.LogError(message, category);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogDebug(string message, string category = "")
        {
            ReportPortal.LogDebug(message, category);
        }

        #endregion

        #region Test Action Logging

        /// <summary>
        /// Log a UI action (click, type, navigate, etc.)
        /// </summary>
        /// <param name="action">The action being performed</param>
        /// <param name="target">The target element or page</param>
        /// <param name="value">Optional value for input actions</param>
        public static void LogAction(string action, string target, string value = "")
        {
            ReportPortal.LogAction(action, target, value);
        }

        /// <summary>
        /// Log a verification/assertion result
        /// </summary>
        /// <param name="verification">Description of what is being verified</param>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="passed">Whether the verification passed</param>
        public static void LogVerification(string verification, string expected = "", string actual = "", bool passed = true)
        {
            ReportPortal.LogVerification(verification, expected, actual, passed);
        }

        /// <summary>
        /// Log test data being used
        /// </summary>
        /// <param name="dataName">Name/description of the data</param>
        /// <param name="dataValue">Value of the data</param>
        /// <param name="category">Optional data category</param>
        public static void LogTestData(string dataName, string dataValue, string category = "")
        {
            ReportPortal.LogTestData(dataName, dataValue, category);
        }

        /// <summary>
        /// Log performance metrics
        /// </summary>
        /// <param name="metricName">Name of the performance metric</param>
        /// <param name="value">Value of the metric</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="threshold">Optional threshold for comparison</param>
        public static void LogPerformance(string metricName, double value, string unit = "ms", double? threshold = null)
        {
            ReportPortal.LogPerformance(metricName, value, unit, threshold);
        }

        /// <summary>
        /// Log configuration information
        /// </summary>
        /// <param name="configName">Name of the configuration</param>
        /// <param name="configValue">Value of the configuration</param>
        public static void LogConfiguration(string configName, string configValue)
        {
            ReportPortal.LogConfiguration(configName, configValue);
        }

        #endregion

        #region Step Management

        /// <summary>
        /// Start a test step with enhanced logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="description">Description of the step</param>
        /// <param name="category">Category of the step</param>
        /// <returns>Step ID for tracking</returns>
        public static string StartStep(string stepName, string description = "", string category = "")
        {
            return StepManager.StartStep(stepName, description, category);
        }

        /// <summary>
        /// End a test step
        /// </summary>
        /// <param name="stepId">Step ID from StartStep</param>
        /// <param name="passed">Whether the step passed</param>
        /// <param name="message">Optional completion message</param>
        public static void EndStep(string stepId, bool passed = true, string message = "")
        {
            var status = passed ? Status.Passed : Status.Failed;
            StepManager.EndStep(stepId, status, message);
        }

        /// <summary>
        /// Execute an action within a step and log it
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="action">Action being performed</param>
        /// <param name="target">Target of the action</param>
        public static void LogStepAction(string stepId, string action, string target = "")
        {
            StepManager.LogAction(stepId, action, target);
        }

        /// <summary>
        /// Log a verification within a step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="verification">Verification description</param>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="passed">Whether verification passed</param>
        public static void LogStepVerification(string stepId, string verification, string expected = "", string actual = "", bool passed = true)
        {
            StepManager.LogVerification(stepId, verification, expected, actual, passed);
        }

        /// <summary>
        /// Log test data within a step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="dataDescription">Description of the data</param>
        /// <param name="data">The data value</param>
        public static void LogStepTestData(string stepId, string dataDescription, string data)
        {
            StepManager.LogTestData(stepId, dataDescription, data);
        }

        #endregion

        #region Custom and Advanced Logging

        /// <summary>
        /// Log a custom message with specified color
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="color">Color for the message</param>
        /// <param name="isBold">Whether the text should be bold</param>
        public static void LogCustom(string message, string color, bool isBold = false)
        {
            ReportPortal.LogCustom(message, color, isBold);
        }

        /// <summary>
        /// Log a section header to organize logs
        /// </summary>
        /// <param name="sectionName">Name of the section</param>
        /// <param name="color">Color for the header</param>
        public static void LogSectionHeader(string sectionName, string color = "darkblue")
        {
            ReportPortal.LogSectionHeader(sectionName, color);
        }

        /// <summary>   
        /// Log a structured table of data
        /// </summary>
        /// <param name="title">Title of the table</param>
        /// <param name="data">Dictionary of key-value pairs</param>
        public static void LogTable(string title, Dictionary<string, string> data)
        {
            ReportPortal.LogTable(title, data);
        }

        /// <summary>
        /// Add a screenshot with formatted logging
        /// </summary>
        /// <param name="screenshotPath">Path to the screenshot</param>
        /// <param name="description">Description of the screenshot</param>
        public static void AddScreenshot(string screenshotPath, string description = "Screenshot")
        {
            ReportPortal.AddScreenshot(screenshotPath, description);
        }

        /// <summary>
        /// Add a file attachment
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="description">Description of the file</param>
        /// <param name="mimeType">MIME type of the file</param>
        public static void AddAttachment(string filePath, string description, string mimeType = "application/octet-stream")
        {
            ReportPortal.AddAttachment(filePath, description, mimeType);
        }

        #endregion

        #region Step Helper Methods

        /// <summary>
        /// Execute a simple action with step logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="action">Action to perform</param>
        /// <param name="category">Step category</param>
        /// <returns>Whether the step passed</returns>
        public static bool ExecuteStep(string stepName, Action action, string category = "Action")
        {
            var stepId = StartStep(stepName, category: category);
            try
            {
                action();
                EndStep(stepId, true, "Step completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                EndStep(stepId, false, $"Step failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Execute an async action with step logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="action">Async action to perform</param>
        /// <param name="category">Step category</param>
        /// <returns>Whether the step passed</returns>
        public static async Task<bool> ExecuteStepAsync(string stepName, Func<Task> action, string category = "Action")
        {
            var stepId = StartStep(stepName, category: category);
            try
            {
                await action();
                EndStep(stepId, true, "Step completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                EndStep(stepId, false, $"Step failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Execute a function with step logging and return result
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="stepName">Name of the step</param>
        /// <param name="function">Function to execute</param>
        /// <param name="category">Step category</param>
        /// <returns>Result of the function</returns>
        public static T ExecuteStepWithResult<T>(string stepName, Func<T> function, string category = "Action")
        {
            var stepId = StartStep(stepName, category: category);
            try
            {
                var result = function();
                EndStep(stepId, true, "Step completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                EndStep(stepId, false, $"Step failed: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Log Level Specific Methods

        /// <summary>
        /// Log a trace message for detailed execution flow
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogTrace(string message, string category = "")
        {
            ReportPortal.LogCustom(message, "gray", false, LogLevel.Trace);
        }

        /// <summary>
        /// Log a fatal error message for critical system failures
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogFatal(string message, string category = "")
        {
            ReportPortal.LogCustom(message, "darkred", true, LogLevel.Fatal);
        }

        /// <summary>
        /// Log a message with a specific log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logLevel">The specific log level to use</param>
        /// <param name="category">Optional category for the log</param>
        public static void LogWithLevel(string message, LogLevel logLevel, string category = "")
        {
            var color = logLevel switch
            {
                LogLevel.Trace => "gray",
                LogLevel.Debug => "blue", 
                LogLevel.Info => "green",
                LogLevel.Warning => "orange",
                LogLevel.Error => "red",
                LogLevel.Fatal => "darkred",
                _ => "black"
            };
            
            ReportPortal.LogCustom(message, color, logLevel >= LogLevel.Warning, logLevel);
        }

        #endregion
    }
}