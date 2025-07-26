using System.Runtime.CompilerServices;
using PlaywrightTestFramework.Reporting;
using PlaywrightTestFramework.Utils;
using ReportPortal.Client.Abstractions.Models;

namespace PlaywrightTestFramework.Extensions
{
    /// <summary>
    /// Extension methods for easy test step management with ReportPortal
    /// </summary>
    public static class TestStepExtensions
    {
        /// <summary>
        /// Execute an action with automatic step logging using method name
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="description">Optional step description</param>
        /// <param name="category">Step category</param>
        /// <param name="methodName">Automatically captured method name</param>
        public static void ExecuteAsStep(this Action action, 
            string description = "", 
            string category = "Action", 
            [CallerMemberName] string methodName = "")
        {
            Logger.ExecuteStep(methodName, action, category);
        }

        /// <summary>
        /// Execute an async action with automatic step logging using method name
        /// </summary>
        /// <param name="action">Async action to execute</param>
        /// <param name="description">Optional step description</param>
        /// <param name="category">Step category</param>
        /// <param name="methodName">Automatically captured method name</param>
        public static async Task ExecuteAsStepAsync(this Func<Task> action, 
            string description = "", 
            string category = "Action", 
            [CallerMemberName] string methodName = "")
        {
            await Logger.ExecuteStepAsync(methodName, action, category);
        }

        /// <summary>
        /// Execute a function with automatic step logging and return result
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="function">Function to execute</param>
        /// <param name="description">Optional step description</param>
        /// <param name="category">Step category</param>
        /// <param name="methodName">Automatically captured method name</param>
        /// <returns>Result of the function</returns>
        public static T ExecuteAsStep<T>(this Func<T> function, 
            string description = "", 
            string category = "Action", 
            [CallerMemberName] string methodName = "")
        {
            return Logger.ExecuteStepWithResult(methodName, function, category);
        }

        /// <summary>
        /// Execute an async function with automatic step logging and return result
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="function">Async function to execute</param>
        /// <param name="description">Optional step description</param>
        /// <param name="category">Step category</param>
        /// <param name="methodName">Automatically captured method name</param>
        /// <returns>Result of the function</returns>
        public static async Task<T> ExecuteAsStepAsync<T>(this Func<Task<T>> function, 
            string description = "", 
            string category = "Action", 
            [CallerMemberName] string methodName = "")
        {
            var stepId = Logger.StartStep(methodName, description, category);
            try
            {
                var result = await function();
                Logger.EndStep(stepId, true, "Step completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                Logger.EndStep(stepId, false, $"Step failed: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Fluent interface for test step management
    /// </summary>
    public class FluentTestStep : IDisposable
    {
        private readonly string _stepId;
        private readonly TestStepManager _stepManager;
        private bool _disposed = false;
        private Status _status = Status.Passed;
        private string _completionMessage = "";

        internal FluentTestStep(string stepId, TestStepManager stepManager)
        {
            _stepId = stepId;
            _stepManager = stepManager;
        }

        /// <summary>
        /// Log an action within this step
        /// </summary>
        /// <param name="action">Action description</param>
        /// <param name="target">Target of the action</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep LogAction(string action, string target = "")
        {
            _stepManager.LogAction(_stepId, action, target);
            return this;
        }

        /// <summary>
        /// Log a verification within this step
        /// </summary>
        /// <param name="verification">Verification description</param>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="passed">Whether verification passed</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep LogVerification(string verification, string expected = "", string actual = "", bool passed = true)
        {
            _stepManager.LogVerification(_stepId, verification, expected, actual, passed);
            if (!passed)
            {
                _status = Status.Failed;
            }
            return this;
        }

        /// <summary>
        /// Log test data within this step
        /// </summary>
        /// <param name="dataDescription">Description of the data</param>
        /// <param name="data">The data value</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep LogTestData(string dataDescription, string data)
        {
            _stepManager.LogTestData(_stepId, dataDescription, data);
            return this;
        }

        /// <summary>
        /// Add a screenshot to this step
        /// </summary>
        /// <param name="screenshotPath">Path to the screenshot</param>
        /// <param name="description">Description of the screenshot</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep AddScreenshot(string screenshotPath, string description = "Screenshot")
        {
            _stepManager.AddScreenshot(_stepId, screenshotPath, description);
            return this;
        }

        /// <summary>
        /// Set the completion status and message for this step
        /// </summary>
        /// <param name="status">Status of the step</param>
        /// <param name="message">Completion message</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep SetResult(Status status, string message = "")
        {
            _status = status;
            _completionMessage = message;
            return this;
        }

        /// <summary>
        /// Mark the step as failed
        /// </summary>
        /// <param name="message">Failure message</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep Failed(string message = "")
        {
            return SetResult(Status.Failed, message);
        }

        /// <summary>
        /// Mark the step as passed
        /// </summary>
        /// <param name="message">Success message</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep Passed(string message = "")
        {
            return SetResult(Status.Passed, message);
        }

        /// <summary>
        /// Mark the step as skipped
        /// </summary>
        /// <param name="message">Skip message</param>
        /// <returns>This instance for chaining</returns>
        public FluentTestStep Skipped(string message = "")
        {
            return SetResult(Status.Skipped, message);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _stepManager.EndStep(_stepId, _status, _completionMessage);
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Factory class for creating fluent test steps
    /// </summary>
    public static class TestStep
    {
        /// <summary>
        /// Start a new test step with fluent interface
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="description">Description of the step</param>
        /// <param name="category">Category of the step</param>
        /// <returns>FluentTestStep instance</returns>
        public static FluentTestStep Start(string stepName, string description = "", string category = "")
        {
            var stepId = Logger.StartStep(stepName, description, category);
            return new FluentTestStep(stepId, Logger.StepManager);
        }

        /// <summary>
        /// Execute an action with fluent step logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="action">Action to execute</param>
        /// <param name="category">Step category</param>
        /// <returns>FluentTestStep instance</returns>
        public static FluentTestStep Execute(string stepName, Action action, string category = "Action")
        {
            var step = Start(stepName, category: category);
            try
            {
                action();
                step.Passed("Action completed successfully");
            }
            catch (Exception ex)
            {
                step.Failed($"Action failed: {ex.Message}");
                throw;
            }
            return step;
        }

        /// <summary>
        /// Execute an async action with fluent step logging
        /// </summary>
        /// <param name="stepName">Name of the step</param>
        /// <param name="action">Async action to execute</param>
        /// <param name="category">Step category</param>
        /// <returns>FluentTestStep instance</returns>
        public static async Task<FluentTestStep> ExecuteAsync(string stepName, Func<Task> action, string category = "Action")
        {
            var step = Start(stepName, category: category);
            try
            {
                await action();
                step.Passed("Action completed successfully");
            }
            catch (Exception ex)
            {
                step.Failed($"Action failed: {ex.Message}");
                throw;
            }
            return step;
        }
    }
}