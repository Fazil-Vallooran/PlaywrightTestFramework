using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace PlaywrightTestFramework.Utils
{
    /// <summary>
    /// Configuration management utility for test framework settings
    /// </summary>
    public static class ConfigurationManager
    {
        private static IConfiguration? _configuration;
        private static readonly object _lock = new object();

        /// <summary>
        /// Get the configuration instance (lazy initialization)
        /// </summary>
        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    lock (_lock)
                    {
                        if (_configuration == null)
                        {
                            InitializeConfiguration();
                        }
                    }
                }
                return _configuration!;
            }
        }

        /// <summary>
        /// Initialize configuration from various sources
        /// </summary>
        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddJsonFile("ReportPortal.config.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs());

            _configuration = builder.Build();
        }

        /// <summary>
        /// Get browser configuration
        /// </summary>
        public static string GetBrowser()
        {
            return Configuration["browser"] ?? Configuration["Browser"] ?? "chromium";
        }

        /// <summary>
        /// Get headless mode setting
        /// </summary>
        public static bool IsHeadless()
        {
            var value = Configuration["headless"] ?? Configuration["Headless"] ?? "true";
            return bool.TryParse(value, out var result) ? result : true;
        }

        /// <summary>
        /// Get base URL for tests
        /// </summary>
        public static string GetBaseUrl()
        {
            return Configuration["baseUrl"] ?? Configuration["BaseUrl"] ?? "https://example.com";
        }

        /// <summary>
        /// Get test timeout in seconds
        /// </summary>
        public static int GetTimeout()
        {
            var value = Configuration["timeout"] ?? Configuration["Timeout"] ?? "30";
            return int.TryParse(value, out var result) ? result : 30;
        }

        /// <summary>
        /// Get parallel execution setting
        /// </summary>
        public static bool IsParallelExecutionEnabled()
        {
            var value = Configuration["parallelExecution"] ?? Configuration["ParallelExecution"] ?? "true";
            return bool.TryParse(value, out var result) ? result : true;
        }

        /// <summary>
        /// Get screenshot on failure setting
        /// </summary>
        public static bool ShouldTakeScreenshotOnFailure()
        {
            var value = Configuration["screenshotOnFailure"] ?? Configuration["ScreenshotOnFailure"] ?? "true";
            return bool.TryParse(value, out var result) ? result : true;
        }

        /// <summary>
        /// Get screenshots directory
        /// </summary>
        public static string GetScreenshotsDirectory()
        {
            return Configuration["screenshotsDirectory"] ?? Configuration["ScreenshotsDirectory"] ?? "Screenshots";
        }

        /// <summary>
        /// Get test data directory
        /// </summary>
        public static string GetTestDataDirectory()
        {
            return Configuration["testDataDirectory"] ?? Configuration["TestDataDirectory"] ?? "TestData";
        }

        /// <summary>
        /// Get ReportPortal enabled setting
        /// </summary>
        public static bool IsReportPortalEnabled()
        {
            var value = Configuration["ReportPortal:enabled"] ?? Configuration["reportPortal:enabled"] ?? "false";
            return bool.TryParse(value, out var result) ? result : false;
        }

        /// <summary>
        /// Get ReportPortal server URL
        /// </summary>
        public static string GetReportPortalUrl()
        {
            return Configuration["ReportPortal:server:url"] ?? Configuration["reportPortal:server:url"] ?? "";
        }

        /// <summary>
        /// Get ReportPortal project name
        /// </summary>
        public static string GetReportPortalProject()
        {
            return Configuration["ReportPortal:server:project"] ?? Configuration["reportPortal:server:project"] ?? "";
        }

        /// <summary>
        /// Get environment name
        /// </summary>
        public static string GetEnvironment()
        {
            return Configuration["environment"] ?? Configuration["Environment"] ?? 
                   Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Development";
        }

        /// <summary>
        /// Get retry count for failed tests
        /// </summary>
        public static int GetRetryCount()
        {
            var value = Configuration["retryCount"] ?? Configuration["RetryCount"] ?? "0";
            return int.TryParse(value, out var result) ? Math.Max(0, result) : 0;
        }

        /// <summary>
        /// Get performance monitoring enabled setting
        /// </summary>
        public static bool IsPerformanceMonitoringEnabled()
        {
            var value = Configuration["performanceMonitoring"] ?? Configuration["PerformanceMonitoring"] ?? "true";
            return bool.TryParse(value, out var result) ? result : true;
        }

        /// <summary>
        /// Get log level setting
        /// </summary>
        public static string GetLogLevel()
        {
            return Configuration["logLevel"] ?? Configuration["LogLevel"] ?? "Info";
        }

        /// <summary>
        /// Get custom configuration value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value</returns>
        public static string GetValue(string key, string defaultValue = "")
        {
            return Configuration[key] ?? defaultValue;
        }

        /// <summary>
        /// Get custom configuration value as specific type
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found or conversion fails</param>
        /// <returns>Configuration value</returns>
        public static T GetValue<T>(string key, T defaultValue = default(T))
        {
            var value = Configuration[key];
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            try
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)value;

                if (typeof(T) == typeof(bool))
                    return (T)(object)bool.Parse(value);

                if (typeof(T) == typeof(int))
                    return (T)(object)int.Parse(value);

                if (typeof(T) == typeof(double))
                    return (T)(object)double.Parse(value);

                if (typeof(T) == typeof(decimal))
                    return (T)(object)decimal.Parse(value);

                if (typeof(T) == typeof(DateTime))
                    return (T)(object)DateTime.Parse(value);

                // For complex types, try JSON deserialization
                return JsonSerializer.Deserialize<T>(value) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get all configuration values as dictionary
        /// </summary>
        /// <returns>Dictionary of all configuration values</returns>
        public static Dictionary<string, string> GetAllValues()
        {
            var result = new Dictionary<string, string>();
            
            foreach (var item in Configuration.AsEnumerable())
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                {
                    result[item.Key] = item.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Log all configuration values for debugging
        /// </summary>
        public static void LogConfiguration()
        {
            Logger.LogSectionHeader("Test Configuration", "darkblue");
            
            var configs = new Dictionary<string, string>
            {
                { "Browser", GetBrowser() },
                { "Headless", IsHeadless().ToString() },
                { "Base URL", GetBaseUrl() },
                { "Timeout", $"{GetTimeout()}s" },
                { "Environment", GetEnvironment() },
                { "Parallel Execution", IsParallelExecutionEnabled().ToString() },
                { "Screenshot on Failure", ShouldTakeScreenshotOnFailure().ToString() },
                { "Screenshots Directory", GetScreenshotsDirectory() },
                { "ReportPortal Enabled", IsReportPortalEnabled().ToString() },
                { "Performance Monitoring", IsPerformanceMonitoringEnabled().ToString() },
                { "Log Level", GetLogLevel() },
                { "Retry Count", GetRetryCount().ToString() }
            };

            foreach (var config in configs)
            {
                Logger.LogConfiguration(config.Key, config.Value);
            }
        }

        /// <summary>
        /// Validate critical configuration values
        /// </summary>
        /// <returns>True if configuration is valid, false otherwise</returns>
        public static bool ValidateConfiguration()
        {
            var issues = new List<string>();

            // Validate browser
            var browser = GetBrowser().ToLower();
            if (!new[] { "chromium", "firefox", "webkit" }.Contains(browser))
            {
                issues.Add($"Invalid browser '{browser}'. Must be one of: chromium, firefox, webkit");
            }

            // Validate timeout
            var timeout = GetTimeout();
            if (timeout <= 0)
            {
                issues.Add($"Invalid timeout '{timeout}'. Must be greater than 0");
            }

            // Validate base URL
            var baseUrl = GetBaseUrl();
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            {
                issues.Add($"Invalid base URL '{baseUrl}'. Must be a valid absolute URL");
            }

            // Validate directories exist or can be created
            try
            {
                var screenshotsDir = GetScreenshotsDirectory();
                if (!Directory.Exists(screenshotsDir))
                {
                    Directory.CreateDirectory(screenshotsDir);
                }

                var testDataDir = GetTestDataDirectory();
                if (!Directory.Exists(testDataDir))
                {
                    Directory.CreateDirectory(testDataDir);
                }
            }
            catch (Exception ex)
            {
                issues.Add($"Error creating directories: {ex.Message}");
            }

            // Log any issues
            if (issues.Any())
            {
                Logger.LogError("Configuration validation failed:");
                foreach (var issue in issues)
                {
                    Logger.LogError($"- {issue}");
                }
                return false;
            }

            Logger.LogInfo("Configuration validation passed successfully");
            return true;
        }
    }
}