using System.Reflection;
using PlaywrightTestFramework.Reporting;

namespace PlaywrightTestFramework.Utils
{
    /// <summary>
    /// Helper class for executing methods marked with TestStep attribute
    /// </summary>
    public static class TestStepExecutor
    {
        /// <summary>
        /// Execute a method with automatic step logging based on TestStep attribute
        /// </summary>
        /// <param name="methodInfo">The method to execute</param>
        /// <param name="instance">The instance to execute the method on</param>
        /// <param name="parameters">Parameters for the method</param>
        /// <returns>The result of the method execution</returns>
        public static async Task<object?> ExecuteWithStepLogging(MethodInfo methodInfo, object instance, params object[] parameters)
        {
            var stepAttribute = methodInfo.GetCustomAttribute<TestStepAttribute>();
            
            if (stepAttribute != null)
            {
                var stepId = ReportPortalHelper.StartTestStep(stepAttribute.StepName, stepAttribute.Description);
                
                try
                {
                    object? result;
                    
                    if (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType.IsSubclassOf(typeof(Task)))
                    {
                        var task = (Task)methodInfo.Invoke(instance, parameters)!;
                        await task;
                        
                        // Get result if it's Task<T>
                        if (methodInfo.ReturnType.IsGenericType)
                        {
                            var property = typeof(Task<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments()[0])
                                                          .GetProperty("Result");
                            result = property?.GetValue(task);
                        }
                        else
                        {
                            result = null;
                        }
                    }
                    else
                    {
                        result = methodInfo.Invoke(instance, parameters);
                    }
                    
                    ReportPortalHelper.EndTestStep(stepId, ReportPortal.Client.Abstractions.Models.Status.Passed, 
                        $"Step '{stepAttribute.StepName}' completed successfully");
                    
                    return result;
                }
                catch (Exception ex)
                {
                    ReportPortalHelper.EndTestStep(stepId, ReportPortal.Client.Abstractions.Models.Status.Failed, 
                        $"Step '{stepAttribute.StepName}' failed: {ex.Message}");
                    throw;
                }
            }
            else
            {
                // Execute without step logging if no attribute is present
                if (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType.IsSubclassOf(typeof(Task)))
                {
                    var task = (Task)methodInfo.Invoke(instance, parameters)!;
                    await task;
                    
                    if (methodInfo.ReturnType.IsGenericType)
                    {
                        var property = typeof(Task<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments()[0])
                                                      .GetProperty("Result");
                        return property?.GetValue(task);
                    }
                    return null;
                }
                else
                {
                    return methodInfo.Invoke(instance, parameters);
                }
            }
        }
    }
}