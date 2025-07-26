using ReportPortal.Shared;
using ReportPortal.Shared.Reporter;
using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using PlaywrightTestFramework.Extra;

namespace PlaywrightTestFramework.Reporting
{
    /// <summary>
    /// Advanced test step manager for ReportPortal with enhanced features
    /// </summary>
    public class TestStepManager
    {
        private readonly Dictionary<string, TestStepContext> _activeSteps = new();
        private readonly ITestReporter? _testReporter;

        public TestStepManager(ITestReporter? testReporter = null)
        {
            _testReporter = testReporter ?? ReportPortalHelper.GetCurrentTestReporter();
        }

        /// <summary>
        /// Start a new test step with enhanced logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="description">Description of the step</param>
        /// <param name="category">Category of the step (e.g., Setup, Action, Verification)</param>
        /// <returns>Step ID for tracking</returns>
        public string StartStep(string stepName, string description = "", string category = "")
        {
            var stepId = Guid.NewGuid().ToString();
            var startTime = DateTime.UtcNow;

            try
            {
                if (_testReporter != null)
                {
                    var stepRequest = new StartTestItemRequest
                    {
                        Name = stepName,
                        Description = description,
                        StartTime = startTime,
                        Type = TestItemType.Step
                    };

                    // Add category as attribute if provided
                    if (!string.IsNullOrEmpty(category))
                    {
                        stepRequest.Attributes = new List<ItemAttribute>
                        {
                            new() { Key = "Category", Value = category }
                        };
                    }

                    var stepReporter = _testReporter.StartChildTestReporter(stepRequest);
                    if (stepReporter != null)
                    {
                        var stepContext = new TestStepContext
                        {
                            StepId = stepId,
                            StepName = stepName,
                            Description = description,
                            Category = category,
                            StartTime = startTime,
                            StepReporter = stepReporter
                        };

                        _activeSteps[stepId] = stepContext;

                        // Log step start with formatted message
                        var startMessage = FontColour.FormatStepStartMessage(stepName);
                        if (!string.IsNullOrEmpty(category))
                        {
                            startMessage += $"<br/>{FontColour.FormatCustomMessage($"Category: {category}", "gray")}";
                        }
                        if (!string.IsNullOrEmpty(description))
                        {
                            startMessage += $"<br/>{FontColour.FormatCustomMessage($"Description: {description}", "gray")}";
                        }

                        stepReporter.Log(new CreateLogItemRequest
                        {
                            Text = startMessage,
                            Level = LogLevel.Info,
                            Time = startTime
                        });
                    }
                }

                ReportPortalHelper.LogInfo(FontColour.FormatStepStartMessage($"[{category}] {stepName}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting test step '{stepName}': {ex.Message}");
            }

            return stepId;
        }

        /// <summary>
        /// End a test step with enhanced logging and timing
        /// </summary>
        /// <param name="stepId">Step ID returned from StartStep</param>
        /// <param name="status">Status of the step</param>
        /// <param name="message">Optional message for the step completion</param>
        public void EndStep(string stepId, Status status = Status.Passed, string message = "")
        {
            try
            {
                if (_activeSteps.TryGetValue(stepId, out var stepContext))
                {
                    var endTime = DateTime.UtcNow;
                    var duration = endTime - stepContext.StartTime;

                    // Log completion message with timing
                    var completionMessage = FontColour.FormatStepEndMessage(stepContext.StepName, status.ToString());
                    completionMessage += $"<br/>{FontColour.FormatPerformanceMessage("Duration", duration.TotalMilliseconds.ToString("F2"), "ms")}";

                    if (!string.IsNullOrEmpty(message))
                    {
                        var messageColor = status switch
                        {
                            Status.Passed => "green",
                            Status.Failed => "red",
                            Status.Skipped => "yellow",
                            _ => "blue"
                        };
                        completionMessage += $"<br/>{FontColour.FormatCustomMessage(message, messageColor)}";
                    }

                    stepContext.StepReporter?.Log(new CreateLogItemRequest
                    {
                        Text = completionMessage,
                        Level = status == Status.Failed ? LogLevel.Error : LogLevel.Info,
                        Time = endTime
                    });

                    stepContext.StepReporter?.Finish(new FinishTestItemRequest
                    {
                        EndTime = endTime,
                        Status = status
                    });

                    _activeSteps.Remove(stepId);

                    // Log to main test reporter as well
                    ReportPortalHelper.LogInfo(FontColour.FormatStepEndMessage($"[{stepContext.Category}] {stepContext.StepName}", status.ToString()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ending test step: {ex.Message}");
            }
        }

        /// <summary>
        /// Log an action within a step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="action">Action being performed</param>
        /// <param name="target">Target of the action</param>
        public void LogAction(string stepId, string action, string target = "")
        {
            if (_activeSteps.TryGetValue(stepId, out var stepContext))
            {
                var actionMessage = FontColour.FormatActionMessage(action, target);
                stepContext.StepReporter?.Log(new CreateLogItemRequest
                {
                    Text = actionMessage,
                    Level = LogLevel.Info,
                    Time = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Log a verification within a step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="verification">Verification being performed</param>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="passed">Whether the verification passed</param>
        public void LogVerification(string stepId, string verification, string expected = "", string actual = "", bool passed = true)
        {
            if (_activeSteps.TryGetValue(stepId, out var stepContext))
            {
                var verificationMessage = FontColour.FormatVerificationMessage(verification, expected, actual);
                if (!passed)
                {
                    verificationMessage = $"<div style='background-color: #ffe6e6; padding: 5px; border-left: 3px solid red;'>{verificationMessage}</div>";
                }

                stepContext.StepReporter?.Log(new CreateLogItemRequest
                {
                    Text = verificationMessage,
                    Level = passed ? LogLevel.Info : LogLevel.Error,
                    Time = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Log test data within a step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="dataDescription">Description of the test data</param>
        /// <param name="data">The test data value</param>
        public void LogTestData(string stepId, string dataDescription, string data)
        {
            if (_activeSteps.TryGetValue(stepId, out var stepContext))
            {
                var dataMessage = FontColour.FormatTestDataMessage(dataDescription, data);
                stepContext.StepReporter?.Log(new CreateLogItemRequest
                {
                    Text = dataMessage,
                    Level = LogLevel.Info,
                    Time = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Add a screenshot to a specific step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <param name="screenshotPath">Path to the screenshot</param>
        /// <param name="description">Description of the screenshot</param>
        public void AddScreenshot(string stepId, string screenshotPath, string description = "Screenshot")
        {
            if (_activeSteps.TryGetValue(stepId, out var stepContext) && File.Exists(screenshotPath))
            {
                try
                {
                    var screenshotData = File.ReadAllBytes(screenshotPath);
                    var screenshotMessage = FontColour.FormatScreenshotMessage(description);

                    stepContext.StepReporter?.Log(new CreateLogItemRequest
                    {
                        Text = screenshotMessage,
                        Level = LogLevel.Info,
                        Time = DateTime.UtcNow,
                        Attach = new LogItemAttach
                        {
                            Name = Path.GetFileName(screenshotPath),
                            MimeType = "image/png",
                            Data = screenshotData
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding screenshot to step: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Get information about an active step
        /// </summary>
        /// <param name="stepId">Step ID</param>
        /// <returns>Step context or null if not found</returns>
        public TestStepContext? GetStepContext(string stepId)
        {
            return _activeSteps.TryGetValue(stepId, out var context) ? context : null;
        }

        /// <summary>
        /// Get all active step IDs
        /// </summary>
        /// <returns>List of active step IDs</returns>
        public List<string> GetActiveStepIds()
        {
            return _activeSteps.Keys.ToList();
        }

        /// <summary>
        /// Clean up all active steps
        /// </summary>
        public void CleanupAllSteps()
        {
            try
            {
                foreach (var stepContext in _activeSteps.Values)
                {
                    stepContext.StepReporter?.Finish(new FinishTestItemRequest
                    {
                        EndTime = DateTime.UtcNow,
                        Status = Status.Skipped
                    });
                }
                _activeSteps.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during step cleanup: {ex.Message}");
                _activeSteps.Clear();
            }
        }
    }

    /// <summary>
    /// Context information for a test step
    /// </summary>
    public class TestStepContext
    {
        public string StepId { get; set; } = string.Empty;
        public string StepName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public ITestReporter? StepReporter { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}