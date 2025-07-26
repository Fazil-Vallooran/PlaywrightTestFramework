using ReportPortal.Shared;
using ReportPortal.Shared.Reporter;
using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using PlaywrightTestFramework.Extra;

namespace PlaywrightTestFramework.Reporting
{
    /// <summary>
    /// Enhanced logger specifically designed for ReportPortal with rich formatting and categorization
    /// </summary>
    public class ReportPortalLogger
    {
        private readonly ITestReporter? _testReporter;

        public ReportPortalLogger(ITestReporter? testReporter = null)
        {
            _testReporter = testReporter ?? ReportPortalHelper.GetCurrentTestReporter();
        }

        #region Basic Logging Methods

        /// <summary>
        /// Log an information message with green formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public void LogInfo(string message, string category = "")
        {
            var formattedMessage = FontColour.FormatInfoMessage(message);
            if (!string.IsNullOrEmpty(category))
            {
                formattedMessage = $"{FontColour.FormatCustomMessage($"[{category}]", "blue", true)} {formattedMessage}";
            }
            
            LogMessage(formattedMessage, LogLevel.Info);
        }

        /// <summary>
        /// Log a warning message with orange/yellow formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public void LogWarning(string message, string category = "")
        {
            var formattedMessage = FontColour.FormatWarnMessage(message);
            if (!string.IsNullOrEmpty(category))
            {
                formattedMessage = $"{FontColour.FormatCustomMessage($"[{category}]", "orange", true)} {formattedMessage}";
            }
            
            LogMessage(formattedMessage, LogLevel.Warning);
        }

        /// <summary>
        /// Log an error message with red formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public void LogError(string message, string category = "")
        {
            var formattedMessage = FontColour.FormatErrorMessage(message);
            if (!string.IsNullOrEmpty(category))
            {
                formattedMessage = $"{FontColour.FormatCustomMessage($"[{category}]", "red", true)} {formattedMessage}";
            }
            
            LogMessage(formattedMessage, LogLevel.Error);
        }

        /// <summary>
        /// Log a debug message with gray formatting
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="category">Optional category for the log</param>
        public void LogDebug(string message, string category = "")
        {
            var formattedMessage = FontColour.FormatDebugMessage(message);
            if (!string.IsNullOrEmpty(category))
            {
                formattedMessage = $"{FontColour.FormatCustomMessage($"[{category}]", "gray", true)} {formattedMessage}";
            }
            
            LogMessage(formattedMessage, LogLevel.Debug);
        }

        #endregion

        #region Specialized Logging Methods

        /// <summary>
        /// Log a test action (click, type, navigate, etc.)
        /// </summary>
        /// <param name="action">The action being performed</param>
        /// <param name="target">The target element or page</param>
        /// <param name="value">Optional value (for input actions)</param>
        public void LogAction(string action, string target, string value = "")
        {
            var message = FontColour.FormatActionMessage(action, target);
            if (!string.IsNullOrEmpty(value))
            {
                message += $"<br/>{FontColour.FormatTestDataMessage("Value", value)}";
            }
            
            LogMessage(message, LogLevel.Info);
        }

        /// <summary>
        /// Log a verification/assertion
        /// </summary>
        /// <param name="verification">Description of what is being verified</param>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="passed">Whether the verification passed</param>
        public void LogVerification(string verification, string expected = "", string actual = "", bool passed = true)
        {
            var message = FontColour.FormatVerificationMessage(verification, expected, actual);
            
            if (!passed)
            {
                message = $"<div style='background-color: #ffe6e6; padding: 10px; border-left: 4px solid red; margin: 5px 0;'>{message}</div>";
            }
            else
            {
                message = $"<div style='background-color: #e6ffe6; padding: 10px; border-left: 4px solid green; margin: 5px 0;'>{message}</div>";
            }
            
            LogMessage(message, passed ? LogLevel.Info : LogLevel.Error);
        }

        /// <summary>
        /// Log test data being used
        /// </summary>
        /// <param name="dataName">Name/description of the data</param>
        /// <param name="dataValue">Value of the data</param>
        /// <param name="category">Optional data category</param>
        public void LogTestData(string dataName, string dataValue, string category = "")
        {
            var message = FontColour.FormatTestDataMessage(dataName, dataValue);
            if (!string.IsNullOrEmpty(category))
            {
                message = $"{FontColour.FormatCustomMessage($"[{category}]", "gold", true)} {message}";
            }
            
            LogMessage(message, LogLevel.Info);
        }

        /// <summary>
        /// Log performance metrics
        /// </summary>
        /// <param name="metricName">Name of the performance metric</param>
        /// <param name="value">Value of the metric</param>
        /// <param name="unit">Unit of measurement (ms, seconds, etc.)</param>
        /// <param name="threshold">Optional threshold for comparison</param>
        public void LogPerformance(string metricName, double value, string unit = "", double? threshold = null)
        {
            var message = FontColour.FormatPerformanceMessage(metricName, value.ToString("F2"), unit);
            
            if (threshold.HasValue)
            {
                var comparison = value <= threshold.Value ? "? Within threshold" : "?? Exceeds threshold";
                var comparisonColor = value <= threshold.Value ? "green" : "orange";
                message += $"<br/>{FontColour.FormatCustomMessage($"Threshold: {threshold.Value} {unit} - {comparison}", comparisonColor)}";
            }
            
            LogMessage(message, LogLevel.Info);
        }

        /// <summary>
        /// Log configuration information
        /// </summary>
        /// <param name="configName">Name of the configuration</param>
        /// <param name="configValue">Value of the configuration</param>
        public void LogConfiguration(string configName, string configValue)
        {
            var message = FontColour.FormatConfigMessage(configName, configValue);
            LogMessage(message, LogLevel.Info);
        }

        /// <summary>
        /// Log a section header to organize logs
        /// </summary>
        /// <param name="sectionName">Name of the section</param>
        /// <param name="color">Color for the header (default: darkblue)</param>
        public void LogSectionHeader(string sectionName, string color = "darkblue")
        {
            var message = FontColour.FormatSectionHeader(sectionName, color);
            LogMessage(message, LogLevel.Info);
        }

        /// <summary>
        /// Log a custom formatted message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="color">Color for the message</param>
        /// <param name="isBold">Whether the message should be bold</param>
        /// <param name="logLevel">Log level (default: Info)</param>
        public void LogCustom(string message, string color, bool isBold = false, LogLevel logLevel = LogLevel.Info)
        {
            var formattedMessage = FontColour.FormatCustomMessage(message, color, isBold);
            LogMessage(formattedMessage, logLevel);
        }

        #endregion

        #region Screenshot and Attachment Methods

        /// <summary>
        /// Add a screenshot with formatted message
        /// </summary>
        /// <param name="screenshotPath">Path to the screenshot file</param>
        /// <param name="description">Description of the screenshot</param>
        /// <param name="logLevel">Log level for the screenshot</param>
        public void AddScreenshot(string screenshotPath, string description = "Screenshot", LogLevel logLevel = LogLevel.Info)
        {
            try
            {
                if (File.Exists(screenshotPath))
                {
                    var screenshotData = File.ReadAllBytes(screenshotPath);
                    var message = FontColour.FormatScreenshotMessage(description);
                    
                    var logRequest = new CreateLogItemRequest
                    {
                        Text = message,
                        Level = logLevel,
                        Time = DateTime.UtcNow,
                        Attach = new LogItemAttach
                        {
                            Name = Path.GetFileName(screenshotPath),
                            MimeType = "image/png",
                            Data = screenshotData
                        }
                    };

                    _testReporter?.Log(logRequest);
                }
                else
                {
                    LogWarning($"Screenshot file not found: {screenshotPath}", "SCREENSHOT");
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to add screenshot: {ex.Message}", "SCREENSHOT");
            }
        }

        /// <summary>
        /// Add a file attachment
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="description">Description of the file</param>
        /// <param name="mimeType">MIME type of the file</param>
        public void AddAttachment(string filePath, string description, string mimeType = "application/octet-stream")
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fileData = File.ReadAllBytes(filePath);
                    var message = FontColour.FormatCustomMessage($"?? ATTACHMENT: {description}", "purple", true);
                    
                    var logRequest = new CreateLogItemRequest
                    {
                        Text = message,
                        Level = LogLevel.Info,
                        Time = DateTime.UtcNow,
                        Attach = new LogItemAttach
                        {
                            Name = Path.GetFileName(filePath),
                            MimeType = mimeType,
                            Data = fileData
                        }
                    };

                    _testReporter?.Log(logRequest);
                }
                else
                {
                    LogWarning($"Attachment file not found: {filePath}", "ATTACHMENT");
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to add attachment: {ex.Message}", "ATTACHMENT");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Log a structured table of data
        /// </summary>
        /// <param name="title">Title of the table</param>
        /// <param name="data">Dictionary of key-value pairs</param>
        public void LogTable(string title, Dictionary<string, string> data)
        {
            var tableHtml = $"<div style='margin: 10px 0;'>" +
                           $"<h4 style='color: {FontColour.GetFontColour("darkblue")}; margin-bottom: 10px;'>{title}</h4>" +
                           $"<table style='border-collapse: collapse; width: 100%; font-family: monospace;'>";

            foreach (var item in data)
            {
                tableHtml += $"<tr>" +
                            $"<td style='border: 1px solid #ddd; padding: 8px; background-color: #f9f9f9; font-weight: bold;'>{item.Key}</td>" +
                            $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Value}</td>" +
                            $"</tr>";
            }

            tableHtml += "</table></div>";
            LogMessage(tableHtml, LogLevel.Info);
        }

        /// <summary>
        /// Internal method to log a message to ReportPortal
        /// </summary>
        /// <param name="message">Formatted message</param>
        /// <param name="logLevel">Log level</param>
        private void LogMessage(string message, LogLevel logLevel)
        {
            try
            {
                var logRequest = new CreateLogItemRequest
                {
                    Text = message,
                    Level = logLevel,
                    Time = DateTime.UtcNow
                };

                _testReporter?.Log(logRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReportPortal logging failed: {ex.Message}");
                Console.WriteLine($"Message: {message}");
            }
        }

        #endregion
    }
}