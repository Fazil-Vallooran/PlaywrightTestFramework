using System.Text.Json;
using PlaywrightTestFramework.Utils;

namespace PlaywrightTestFramework.Utils
{
    /// <summary>
    /// Test data management utility for loading and managing test data from various sources
    /// </summary>
    public static class TestDataManager
    {
        private static readonly Dictionary<string, object> _testDataCache = new();
        private static readonly object _lock = new object();

        /// <summary>
        /// Load test data from JSON file
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="fileName">JSON file name (without extension)</param>
        /// <returns>Deserialized test data</returns>
        public static T LoadFromJson<T>(string fileName)
        {
            var cacheKey = $"json_{fileName}_{typeof(T).Name}";
            
            lock (_lock)
            {
                if (_testDataCache.TryGetValue(cacheKey, out var cachedData))
                {
                    return (T)cachedData;
                }

                var testDataDir = ConfigurationManager.GetTestDataDirectory();
                var filePath = Path.Combine(testDataDir, $"{fileName}.json");

                if (!File.Exists(filePath))
                {
                    Logger.LogError($"Test data file not found: {filePath}");
                    throw new FileNotFoundException($"Test data file not found: {filePath}");
                }

                try
                {
                    var jsonContent = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    });

                    if (data != null)
                    {
                        _testDataCache[cacheKey] = data;
                        Logger.LogInfo($"Test data loaded from: {filePath}");
                        return data;
                    }

                    throw new InvalidOperationException($"Failed to deserialize test data from: {filePath}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading test data from {filePath}: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Load test data from CSV file
        /// </summary>
        /// <param name="fileName">CSV file name (without extension)</param>
        /// <param name="hasHeader">Whether the CSV has a header row</param>
        /// <returns>List of string arrays representing rows</returns>
        public static List<string[]> LoadFromCsv(string fileName, bool hasHeader = true)
        {
            var cacheKey = $"csv_{fileName}";
            
            lock (_lock)
            {
                if (_testDataCache.TryGetValue(cacheKey, out var cachedData))
                {
                    return (List<string[]>)cachedData;
                }

                var testDataDir = ConfigurationManager.GetTestDataDirectory();
                var filePath = Path.Combine(testDataDir, $"{fileName}.csv");

                if (!File.Exists(filePath))
                {
                    Logger.LogError($"Test data file not found: {filePath}");
                    throw new FileNotFoundException($"Test data file not found: {filePath}");
                }

                try
                {
                    var lines = File.ReadAllLines(filePath);
                    var data = new List<string[]>();

                    var startIndex = hasHeader ? 1 : 0;
                    for (int i = startIndex; i < lines.Length; i++)
                    {
                        var row = ParseCsvLine(lines[i]);
                        if (row.Length > 0 && !string.IsNullOrWhiteSpace(string.Join("", row)))
                        {
                            data.Add(row);
                        }
                    }

                    _testDataCache[cacheKey] = data;
                    Logger.LogInfo($"Test data loaded from CSV: {filePath} ({data.Count} rows)");
                    return data;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading CSV data from {filePath}: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Load test data from CSV file as dictionary objects
        /// </summary>
        /// <param name="fileName">CSV file name (without extension)</param>
        /// <returns>List of dictionaries with column headers as keys</returns>
        public static List<Dictionary<string, string>> LoadFromCsvAsDictionaries(string fileName)
        {
            var testDataDir = ConfigurationManager.GetTestDataDirectory();
            var filePath = Path.Combine(testDataDir, $"{fileName}.csv");

            if (!File.Exists(filePath))
            {
                Logger.LogError($"Test data file not found: {filePath}");
                throw new FileNotFoundException($"Test data file not found: {filePath}");
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 2)
                {
                    throw new InvalidOperationException("CSV file must have at least a header row and one data row");
                }

                var headers = ParseCsvLine(lines[0]);
                var data = new List<Dictionary<string, string>>();

                for (int i = 1; i < lines.Length; i++)
                {
                    var row = ParseCsvLine(lines[i]);
                    if (row.Length > 0 && !string.IsNullOrWhiteSpace(string.Join("", row)))
                    {
                        var dictionary = new Dictionary<string, string>();
                        for (int j = 0; j < Math.Min(headers.Length, row.Length); j++)
                        {
                            dictionary[headers[j]] = row[j];
                        }
                        data.Add(dictionary);
                    }
                }

                Logger.LogInfo($"Test data loaded from CSV as dictionaries: {filePath} ({data.Count} rows)");
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading CSV data as dictionaries from {filePath}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get test data for specific test method from configuration or data files
        /// </summary>
        /// <param name="testMethodName">Name of the test method</param>
        /// <param name="dataKey">Specific data key within the test data</param>
        /// <returns>Test data value or empty string if not found</returns>
        public static string GetTestData(string testMethodName, string dataKey)
        {
            try
            {
                // Try to get from configuration first
                var configValue = ConfigurationManager.GetValue($"TestData:{testMethodName}:{dataKey}");
                if (!string.IsNullOrEmpty(configValue))
                {
                    Logger.LogTestData($"{testMethodName}.{dataKey}", configValue);
                    return configValue;
                }

                // Try to load from JSON file named after the test method
                try
                {
                    var testData = LoadFromJson<Dictionary<string, object>>(testMethodName);
                    if (testData.TryGetValue(dataKey, out var value))
                    {
                        var stringValue = value?.ToString() ?? "";
                        Logger.LogTestData($"{testMethodName}.{dataKey}", stringValue);
                        return stringValue;
                    }
                }
                catch
                {
                    // Ignore if file doesn't exist
                }

                Logger.LogWarn($"Test data not found for {testMethodName}.{dataKey}");
                return "";
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting test data for {testMethodName}.{dataKey}: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Get all test data for a specific test method
        /// </summary>
        /// <param name="testMethodName">Name of the test method</param>
        /// <returns>Dictionary of test data</returns>
        public static Dictionary<string, string> GetAllTestData(string testMethodName)
        {
            try
            {
                // Try to load from JSON file first
                try
                {
                    var jsonData = LoadFromJson<Dictionary<string, object>>(testMethodName);
                    var result = new Dictionary<string, string>();
                    
                    foreach (var kvp in jsonData)
                    {
                        result[kvp.Key] = kvp.Value?.ToString() ?? "";
                    }

                    Logger.LogInfo($"Test data loaded for {testMethodName}: {result.Count} items");
                    return result;
                }
                catch
                {
                    // Try to get from configuration
                    var configSection = $"TestData:{testMethodName}";
                    var configData = new Dictionary<string, string>();
                    
                    foreach (var kvp in ConfigurationManager.GetAllValues())
                    {
                        if (kvp.Key.StartsWith(configSection + ":"))
                        {
                            var dataKey = kvp.Key.Substring(configSection.Length + 1);
                            configData[dataKey] = kvp.Value;
                        }
                    }

                    if (configData.Any())
                    {
                        Logger.LogInfo($"Test data loaded from configuration for {testMethodName}: {configData.Count} items");
                        return configData;
                    }
                }

                Logger.LogWarn($"No test data found for {testMethodName}");
                return new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting all test data for {testMethodName}: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Save test data to JSON file
        /// </summary>
        /// <typeparam name="T">Type of data to save</typeparam>
        /// <param name="fileName">File name (without extension)</param>
        /// <param name="data">Data to save</param>
        public static void SaveToJson<T>(string fileName, T data)
        {
            try
            {
                var testDataDir = ConfigurationManager.GetTestDataDirectory();
                if (!Directory.Exists(testDataDir))
                {
                    Directory.CreateDirectory(testDataDir);
                }

                var filePath = Path.Combine(testDataDir, $"{fileName}.json");
                var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                File.WriteAllText(filePath, jsonString);
                Logger.LogInfo($"Test data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error saving test data to {fileName}.json: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Clear test data cache
        /// </summary>
        public static void ClearCache()
        {
            lock (_lock)
            {
                _testDataCache.Clear();
                Logger.LogInfo("Test data cache cleared");
            }
        }

        /// <summary>
        /// Generate random test data based on template
        /// </summary>
        /// <param name="template">Template for data generation</param>
        /// <returns>Generated test data</returns>
        public static Dictionary<string, string> GenerateRandomData(Dictionary<string, string> template)
        {
            var random = new Random();
            var result = new Dictionary<string, string>();

            foreach (var kvp in template)
            {
                var value = kvp.Value.ToLower() switch
                {
                    "email" => GenerateRandomEmail(),
                    "phone" => GenerateRandomPhone(),
                    "name" => GenerateRandomName(),
                    "password" => GenerateRandomPassword(),
                    "number" => random.Next(1000, 9999).ToString(),
                    "date" => DateTime.Now.AddDays(random.Next(-365, 365)).ToString("yyyy-MM-dd"),
                    "text" => GenerateRandomText(10),
                    _ => kvp.Value
                };

                result[kvp.Key] = value;
            }

            Logger.LogInfo($"Generated random test data: {result.Count} items");
            return result;
        }

        #region Private Helper Methods

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString().Trim());
            return result.ToArray();
        }

        private static string GenerateRandomEmail()
        {
            var domains = new[] { "example.com", "test.com", "sample.org" };
            var names = new[] { "user", "test", "demo", "sample" };
            var random = new Random();
            
            return $"{names[random.Next(names.Length)]}{random.Next(100, 999)}@{domains[random.Next(domains.Length)]}";
        }

        private static string GenerateRandomPhone()
        {
            var random = new Random();
            return $"+1{random.Next(100, 999)}{random.Next(100, 999)}{random.Next(1000, 9999)}";
        }

        private static string GenerateRandomName()
        {
            var firstNames = new[] { "John", "Jane", "Bob", "Alice", "Charlie", "Diana" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia" };
            var random = new Random();
            
            return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Attribute for marking test methods with test data file associations
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestDataAttribute : Attribute
    {
        public string FileName { get; }
        public string DataKey { get; }

        public TestDataAttribute(string fileName, string dataKey = "")
        {
            FileName = fileName;
            DataKey = dataKey;
        }
    }
}