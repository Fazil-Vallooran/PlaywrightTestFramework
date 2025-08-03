# Playwright Test Framework

A comprehensive .NET 8.0 test automation framework built with Playwright, NUnit, and ReportPortal integration for modern web application testing.

## 🚀 Features

### Core Framework Features
- **Cross-Browser Testing**: Support for Chromium, Firefox, and WebKit
- **Page Object Model**: Structured, maintainable test architecture
- **Rich Reporting**: Integration with ReportPortal for detailed test reports
- **Enhanced Logging**: Multi-level logging with HTML formatting and visual indicators
- **Test Prioritization**: Built-in test priority system (Critical, High, Medium, Low, Optional)
- **Parallel Execution**: NUnit parallel test execution support
- **Screenshot on Failure**: Automatic screenshot capture for failed tests
- **Step-by-Step Execution**: Detailed test step tracking and reporting

### Advanced Features
- **Custom Test Attributes**: Priority-based test categorization
- **Visual HTML Logging**: Rich, colored log messages with icons and formatting
- **Performance Monitoring**: Built-in performance metrics logging
- **Configuration Management**: Centralized test configuration
- **Error Handling**: Comprehensive error handling and recovery
- **Test Data Management**: Structured test data logging and management

## 📋 Prerequisites

- **.NET 8.0 SDK** or later
- **Visual Studio 2022** or **VS Code** (recommended)
- **Git** for version control

## 🛠️ Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/Fazil-Vallooran/PlaywrightTestFramework.git
cd PlaywrightTestFramework
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Install Playwright Browsers
```bash
# Install required browsers for Playwright
dotnet run playwright install
```
or if using PowerShell/Command Prompt:
```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### 4. Build the Project
```bash
dotnet build
```

## 🧪 Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Specific Categories
```bash
# Run only Smoke tests
dotnet test --filter TestCategory=Smoke

# Run only Functional tests
dotnet test --filter TestCategory=Functional
```

### Run Tests by Priority
```bash
# Run only Critical and High priority tests
dotnet test --filter "Priority=Critical|Priority=High"
```

### Run Tests with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests with Custom Settings
```bash
# Use specific run settings
dotnet test --settings RunSettings.runsettings

# Use priority-specific settings
dotnet test --settings PriorityTest.runsettings
```

## 📁 Project Structure

```
PlaywrightTestFramework/
├── Elements/                 # UI element wrappers
│   ├── BaseElements.cs
│   ├── Button.cs
│   └── TextBox.cs
├── Extensions/               # Framework extensions
│   └── TestStepExtensions.cs
├── Pages/                    # Page Object Model
│   ├── BaseWorkspace.cs
│   └── LoginPage.cs
├── Reporting/               # ReportPortal integration
│   ├── ReportPortalHelper.cs
│   ├── ReportPortalLogger.cs
│   ├── TestStepAttribute.cs
│   └── TestStepManager.cs
├── tests/                   # Test classes
│   ├── BaseTest.cs          # Base test class
│   ├── UnitTest1.cs         # Example tests
│   ├── HtmlFormattingTest.cs
│   ├── LoggingDebugTest.cs
│   └── PriorityDemoTests.cs
├── Utils/                   # Utility classes
│   ├── Logger.cs            # Enhanced logging utility
│   └── TestStepExecutor.cs
├── Images/                  # Test artifacts storage
├── PlaywrightTestFramework.csproj
├── RunSettings.runsettings  # Test execution settings
├── PriorityTest.runsettings # Priority-based settings
├── ReportPortal.config.json # ReportPortal configuration
└── docker-compose.yml       # Docker setup for ReportPortal
```

## 📝 Writing Tests

### Basic Test Structure

```csharp
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MyTests : BaseTest
{
    [Test]
    [Category("Smoke")]
    [Priority(TestPriority.High)]
    [Description("Verify basic page functionality")]
    public async Task MyTest()
    {
        // Test implementation using ExecuteStepAsync for structured logging
        await ExecuteStepAsync("Navigate to page", async () =>
        {
            await Page.GotoAsync("https://example.com");
            Logger.LogAction("Navigate", "https://example.com");
        });

        await ExecuteStepAsync("Verify page title", async () =>
        {
            await Expect(Page).ToHaveTitleAsync("Expected Title");
            Logger.LogVerification("Page title", "Expected Title", await Page.TitleAsync(), true);
        });
    }
}
```

### Test Priorities

Use the `[Priority]` attribute to categorize tests:

```csharp
[Priority(TestPriority.Critical)]  // Must pass - blocks release
[Priority(TestPriority.High)]      // Core functionality
[Priority(TestPriority.Medium)]    // Important features
[Priority(TestPriority.Low)]       // Nice-to-have features
[Priority(TestPriority.Optional)]  // Edge cases
```

### Enhanced Logging

The framework provides comprehensive logging capabilities:

```csharp
// Basic logging
Logger.LogInfo("Test information");
Logger.LogWarn("Warning message");
Logger.LogError("Error message");

// Action logging
Logger.LogAction("Click", "Submit button");
Logger.LogAction("Type", "Username field", "testuser");

// Verification logging
Logger.LogVerification("User logged in", "Dashboard", actualPage, true);

// Test data logging
Logger.LogTestData("Username", "testuser");
Logger.LogTestData("Environment", "staging");

// Performance logging
Logger.LogPerformance("Page load time", 1500, "ms", 2000);
```

### Step-by-Step Execution

Use `ExecuteStepAsync` for structured test execution:

```csharp
await ExecuteStepAsync("Step name", async () =>
{
    // Step implementation
    await Page.ClickAsync("#button");
    Logger.LogAction("Click", "Submit button");
}, "Optional step description");
```

## 🔧 Configuration

### ReportPortal Setup

1. Configure ReportPortal settings in `ReportPortal.config.json`:

```json
{
  "enabled": true,
  "server": {
    "url": "https://your-reportportal-instance.com",
    "project": "your_project",
    "authentication": {
      "uuid": "your-uuid-here"
    }
  },
  "launch": {
    "name": "Playwright Tests",
    "description": "Automated test execution",
    "debugMode": false,
    "attributes": ["playwright", "automation"]
  }
}
```

2. Start ReportPortal using Docker:

```bash
docker-compose up -d
```

### Test Settings

Customize test execution in `RunSettings.runsettings`:

```xml
<RunSettings>
  <TestRunParameters>
    <Parameter name="browser" value="chromium" />
    <Parameter name="headless" value="true" />
    <Parameter name="baseUrl" value="https://example.com" />
  </TestRunParameters>
</RunSettings>
```

## 🏗️ Page Object Model

### Base Page Structure

```csharp
public class LoginPage : BaseWorkspace
{
    public LoginPage(IPage page) : base(page) { }

    private readonly string _usernameInput = "#username";
    private readonly string _passwordInput = "#password";
    private readonly string _loginButton = "#login";

    public async Task LoginAsync(string username, string password)
    {
        await Page.FillAsync(_usernameInput, username);
        await Page.FillAsync(_passwordInput, password);
        await Page.ClickAsync(_loginButton);
    }
}
```

### Element Wrappers

Use enhanced element wrappers for better maintainability:

```csharp
var submitButton = new Button(Page, "#submit", "Submit Button");
await submitButton.ClickAsync();

var usernameField = new TextBox(Page, "#username", "Username Field");
await usernameField.TypeAsync("testuser");
```

## 📊 Reporting & Analytics

### ReportPortal Features
- **Test Execution Dashboard**: Real-time test execution monitoring
- **HTML Formatted Logs**: Rich, visual log messages with colors and icons
- **Screenshots on Failure**: Automatic screenshot capture and attachment
- **Step-by-Step Tracking**: Detailed test step execution and timing
- **Priority-based Analysis**: Test results categorized by priority levels
- **Performance Metrics**: Built-in performance monitoring and reporting

### Log Levels and Filtering

The framework supports multiple log levels:
- **Trace**: Detailed execution flow
- **Debug**: Development and troubleshooting information
- **Info**: General information messages
- **Warning**: Non-critical issues
- **Error**: Error conditions
- **Fatal**: Critical system failures

## 🔄 CI/CD Integration

### GitHub Actions Example

```yaml
name: Playwright Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Install Playwright
      run: dotnet run playwright install --with-deps
    - name: Build
      run: dotnet build --no-restore
    - name: Run tests
      run: dotnet test --no-build --verbosity normal
```

## 🐛 Troubleshooting

### Common Issues

1. **Browser Installation**:
   ```bash
   # If browsers are not installed
   dotnet run playwright install
   ```

2. **Test Discovery Issues**:
   ```bash
   # Clean and rebuild
   dotnet clean
   dotnet build
   ```

3. **ReportPortal Connection**:
   - Verify `ReportPortal.config.json` settings
   - Check network connectivity to ReportPortal instance
   - Validate authentication credentials

4. **Screenshot Issues**:
   - Ensure `Images/` directory exists and is writable
   - Check disk space availability
   - Verify browser is running in headed mode for screenshot capture

### Debug Mode

Enable detailed logging for troubleshooting:

```csharp
Logger.LogDebug("Detailed debug information");
Logger.LogTrace("Trace-level execution details");
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the troubleshooting section above
- Review the example tests for usage patterns

## 🔄 Version History

- **v1.0.0**: Initial release with basic Playwright + NUnit integration
- **v1.1.0**: Added ReportPortal integration and enhanced logging
- **v1.2.0**: Added test prioritization and HTML formatting
- **v1.3.0**: Enhanced error handling and documentation improvements

---

Happy Testing! 🎯