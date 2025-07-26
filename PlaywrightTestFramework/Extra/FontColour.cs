using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace PlaywrightTestFramework.Extra
{
    public static class FontColour
    {
        // Enhanced color palette with more colors
        public static string GetFontColour(string colourName)
        {
            return colourName.ToLower() switch
            {
                "black" => "#000000",
                "white" => "#FFFFFF",
                "red" => "#FF0000",
                "green" => "#00FF00",
                "blue" => "#0000FF",
                "yellow" => "#FFFF00",
                "cyan" => "#00FFFF",
                "magenta" => "#FF00FF",
                "orange" => "#FFA500",
                "purple" => "#800080",
                "gray" => "#808080",
                "lightgray" => "#D3D3D3",
                "darkred" => "#8B0000",
                "darkgreen" => "#006400",
                "darkblue" => "#00008B",
                "lightblue" => "#ADD8E6",
                "lightgreen" => "#90EE90",
                "pink" => "#FFC0CB",
                "brown" => "#A52A2A",
                "silver" => "#C0C0C0",
                "gold" => "#FFD700",
                _ => throw new ArgumentException($"Unknown colour: {colourName}")
            };
        }

        // Legacy methods for backward compatibility
        public static string applyRedFontColour()
        {
            return "<br/><b color=\"red\">";
        }
        
        public static string applyGreenFontColour()
        {
            return "<br/><b color=\"green\">";
        }
        
        public static string applyBlueFontColour()
        {
            return "<br/><b color=\"blue\">";
        }

        // ReportPortal specific formatting methods for different log levels
        
        /// <summary>
        /// Format an info message with green color for ReportPortal
        /// </summary>
        /// <param name="message">The message to format</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatInfoMessage(string message)
        {
            return $"<div style='color: {GetFontColour("green")}; font-weight: bold;'>" +
                   $"<span style='color: {GetFontColour("blue")};'>[INFO]</span> {message}" +
                   "</div>";
        }

        /// <summary>
        /// Format a warning message with yellow/orange color for ReportPortal
        /// </summary>
        /// <param name="message">The message to format</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatWarnMessage(string message)
        {
            return $"<div style='color: {GetFontColour("orange")}; font-weight: bold;'>" +
                   $"<span style='color: {GetFontColour("yellow")};'>[WARN]</span> {message}" +
                   "</div>";
        }

        /// <summary>
        /// Format an error message with red color for ReportPortal
        /// </summary>
        /// <param name="message">The message to format</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatErrorMessage(string message)
        {
            return $"<div style='color: {GetFontColour("red")}; font-weight: bold;'>" +
                   $"<span style='background-color: {GetFontColour("red")}; color: {GetFontColour("white")}; padding: 2px 4px;'>[ERROR]</span> {message}" +
                   "</div>";
        }

        /// <summary>
        /// Format a debug message with gray color for ReportPortal
        /// </summary>
        /// <param name="message">The message to format</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatDebugMessage(string message)
        {
            return $"<div style='color: #808080; font-style: italic;'>" +
                   $"<span style='color: #666666;'>[DEBUG]</span> {message}" +
                   "</div>";
        }

        /// <summary>
        /// Format a step start message with blue color for ReportPortal
        /// </summary>
        /// <param name="stepName">The step name</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatStepStartMessage(string stepName)
        {
            return $"<div style='color: {GetFontColour("blue")}; font-weight: bold; border-left: 3px solid {GetFontColour("blue")}; padding-left: 10px;'>" +
                   $"🔵 <strong>STEP STARTED:</strong> {stepName}" +
                   "</div>";
        }

        /// <summary>
        /// Format a step end message with appropriate color based on status
        /// </summary>
        /// <param name="stepName">The step name</param>
        /// <param name="status">The step status (Passed, Failed, Skipped)</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatStepEndMessage(string stepName, string status)
        {
            var (color, icon) = status.ToUpper() switch
            {
                "PASSED" => (GetFontColour("green"), "✅"),
                "FAILED" => (GetFontColour("red"), "❌"),
                "SKIPPED" => (GetFontColour("yellow"), "⚠️"),
                _ => (GetFontColour("blue"), "🔵")
            };

            return $"<div style='color: {color}; font-weight: bold; border-left: 3px solid {color}; padding-left: 10px;'>" +
                   $"{icon} <strong>STEP ENDED:</strong> {stepName} - <span style='text-transform: uppercase;'>{status}</span>" +
                   "</div>";
        }

        /// <summary>
        /// Format a custom colored message for ReportPortal
        /// </summary>
        /// <param name="message">The message to format</param>
        /// <param name="colorName">The color name</param>
        /// <param name="isBold">Whether the text should be bold</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatCustomMessage(string message, string colorName, bool isBold = false)
        {
            var fontWeight = isBold ? "bold" : "normal";
            var color = GetFontColour(colorName);
            return $"<div style='color: {color}; font-weight: {fontWeight};'>{message}</div>";
        }

        /// <summary>
        /// Format a test action message with cyan color and action icon
        /// </summary>
        /// <param name="action">The action being performed</param>
        /// <param name="target">The target of the action</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatActionMessage(string action, string target = "")
        {
            var targetText = !string.IsNullOrEmpty(target) ? $" on '{target}'" : "";
            return $"<div style='color: {GetFontColour("cyan")}; font-weight: bold;'>" +
                   $"🎯 <strong>ACTION:</strong> {action}{targetText}" +
                   "</div>";
        }

        /// <summary>
        /// Format a verification message with purple color
        /// </summary>
        /// <param name="verification">The verification being performed</param>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatVerificationMessage(string verification, string expected = "", string actual = "")
        {
            var details = "";
            if (!string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(actual))
            {
                details = $"<br/><small>Expected: <em>{expected}</em> | Actual: <em>{actual}</em></small>";
            }
            
            return $"<div style='color: {GetFontColour("purple")}; font-weight: bold;'>" +
                   $"🔍 <strong>VERIFICATION:</strong> {verification}{details}" +
                   "</div>";
        }

        /// <summary>
        /// Format a test data message with gold color
        /// </summary>
        /// <param name="dataDescription">Description of the test data</param>
        /// <param name="data">The test data value</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatTestDataMessage(string dataDescription, string data)
        {
            return $"<div style='color: {GetFontColour("gold")}; font-weight: bold;'>" +
                   $"📊 <strong>TEST DATA:</strong> {dataDescription}: <code style='background-color: #f0f0f0; padding: 2px 4px;'>{data}</code>" +
                   "</div>";
        }

        /// <summary>
        /// Format a screenshot attachment message
        /// </summary>
        /// <param name="description">Description of the screenshot</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatScreenshotMessage(string description)
        {
            return $"<div style='color: {GetFontColour("darkblue")}; font-weight: bold;'>" +
                   $"📸 <strong>SCREENSHOT:</strong> {description}" +
                   "</div>";
        }

        /// <summary>
        /// Format a section header message with larger font and border
        /// </summary>
        /// <param name="sectionName">Name of the section</param>
        /// <param name="colorName">Color for the section header</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatSectionHeader(string sectionName, string colorName = "darkblue")
        {
            var color = GetFontColour(colorName);
            return $"<div style='color: {color}; font-size: 16px; font-weight: bold; border-bottom: 2px solid {color}; padding-bottom: 5px; margin: 10px 0;'>" +
                   $"📋 {sectionName.ToUpper()}" +
                   "</div>";
        }

        /// <summary>
        /// Format a performance metric message
        /// </summary>
        /// <param name="metricName">Name of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatPerformanceMessage(string metricName, string value, string unit = "")
        {
            var unitText = !string.IsNullOrEmpty(unit) ? $" {unit}" : "";
            return $"<div style='color: {GetFontColour("brown")}; font-weight: bold;'>" +
                   $"⏱️ <strong>PERFORMANCE:</strong> {metricName}: <span style='color: {GetFontColour("darkgreen")};'>{value}{unitText}</span>" +
                   "</div>";
        }

        /// <summary>
        /// Format a configuration message with silver color
        /// </summary>
        /// <param name="configName">Name of the configuration</param>
        /// <param name="configValue">Value of the configuration</param>
        /// <returns>HTML formatted message for ReportPortal</returns>
        public static string FormatConfigMessage(string configName, string configValue)
        {
            return $"<div style='color: {GetFontColour("silver")}; font-weight: bold;'>" +
                   $"⚙️ <strong>CONFIG:</strong> {configName}: <code style='background-color: #f0f0f0; padding: 2px 4px;'>{configValue}</code>" +
                   "</div>";
        }
    }
}
