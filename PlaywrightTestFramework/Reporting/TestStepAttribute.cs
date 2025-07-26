using System;

namespace PlaywrightTestFramework.Reporting
{
    /// <summary>
    /// Attribute to mark methods as test steps for ReportPortal logging
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestStepAttribute : Attribute
    {
        public string StepName { get; }
        public string Description { get; }

        public TestStepAttribute(string stepName, string description = "")
        {
            StepName = stepName;
            Description = description;
        }
    }
}