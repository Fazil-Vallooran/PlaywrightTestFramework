# ReportPortal Log Level Filtering Guide

## Overview
This document explains how to use different log levels in the Playwright Test Framework for effective filtering and analysis in ReportPortal.

## Available Log Levels

### 1. TRACE (LogLevel.Trace)
- **Purpose**: Detailed execution flow tracking
- **Use Case**: Debugging complex workflows, understanding step-by-step execution
- **Example**: Method entry/exit, detailed process tracking
- **Color**: Gray
- **When to Use**: Development phase, complex debugging scenarios

```csharp
LogWithLevel("Entering page responsiveness verification phase", LogLevel.Trace, "Verification Flow");
```

### 2. DEBUG (LogLevel.Debug)
- **Purpose**: Technical debugging information
- **Use Case**: Development debugging, technical details
- **Example**: Variable values, technical state information
- **Color**: Blue
- **When to Use**: Development, troubleshooting technical issues

```csharp
LogWithLevel("Retrieved page title: 'Playwright'", LogLevel.Debug, "Title Data");
```

### 3. INFO (LogLevel.Info)
- **Purpose**: General informational messages
- **Use Case**: Standard test execution information
- **Example**: Test actions, successful operations
- **Color**: Green
- **When to Use**: Normal test execution, general information

```csharp
LogWithLevel("? Page loaded successfully and URL verified", LogLevel.Info, "Verification");
```

### 4. WARNING (LogLevel.Warning)
- **Purpose**: Important alerts and notifications
- **Use Case**: Non-critical issues, attention-needed situations
- **Example**: Missing optional features, performance warnings
- **Color**: Orange
- **When to Use**: Non-blocking issues, important notifications

```csharp
LogWithLevel("?? Documentation link not found - non-critical", LogLevel.Warning, "Missing Feature");
```

### 5. ERROR (LogLevel.Error)
- **Purpose**: Error conditions and failures
- **Use Case**: Test failures, blocking issues
- **Example**: Failed assertions, critical errors
- **Color**: Red
- **When to Use**: Test failures, blocking errors

```csharp
LogWithLevel("? Poor performance detected", LogLevel.Error, "Performance Issue");
```

### 6. FATAL (LogLevel.Fatal)
- **Purpose**: Critical system failures
- **Use Case**: System-level failures, critical errors
- **Example**: Infrastructure failures, critical system errors
- **Color**: Dark Red
- **When to Use**: System crashes, critical infrastructure failures

```csharp
LogWithLevel("?? Critical system failure detected", LogLevel.Fatal, "System Error");
```

## Filtering Scenarios in ReportPortal

### Scenario 1: Development & Debugging
**Filter**: TRACE + DEBUG + INFO + WARNING + ERROR + FATAL
**Purpose**: See everything for comprehensive debugging
```
Use when: Active development, complex debugging
ReportPortal Filter: Show all levels
```

### Scenario 2: Production Monitoring
**Filter**: INFO + WARNING + ERROR + FATAL
**Purpose**: Monitor production systems without debug noise
```
Use when: Production monitoring, operational oversight
ReportPortal Filter: Hide TRACE and DEBUG
```

### Scenario 3: Error Investigation
**Filter**: ERROR + FATAL only
**Purpose**: Focus on failures and critical issues
```
Use when: Investigating test failures, error analysis
ReportPortal Filter: Show only ERROR and FATAL
```

### Scenario 4: Health Monitoring
**Filter**: WARNING + ERROR + FATAL
**Purpose**: Monitor system health and potential issues
```
Use when: System health checks, proactive monitoring
ReportPortal Filter: Show WARNING, ERROR, and FATAL
```

### Scenario 5: Clean Test Reports
**Filter**: INFO + WARNING + ERROR + FATAL
**Purpose**: Clean reports for stakeholders without debug details
```
Use when: Management reports, clean test summaries
ReportPortal Filter: Hide TRACE and DEBUG
```

## Implementation Examples

### Basic Usage
```csharp
// Use appropriate log level for the message type
LogWithLevel("Starting test execution", LogLevel.Info, "Test Flow");
LogWithLevel("Debug: Element located", LogLevel.Debug, "UI Debug");
LogWithLevel("Warning: Optional feature missing", LogLevel.Warning, "Feature Check");
LogWithLevel("Error: Test assertion failed", LogLevel.Error, "Test Failure");
```

### Context-Aware Logging
```csharp
// Different log levels based on test outcome
if (testPassed)
{
    LogWithLevel("? Test completed successfully", LogLevel.Info, "Result");
}
else
{
    LogWithLevel("? Test failed with errors", LogLevel.Error, "Result");
}
```

### Performance-Based Logging
```csharp
var loadTime = GetPageLoadTime();
if (loadTime < 2000)
{
    LogWithLevel($"? Excellent performance: {loadTime}ms", LogLevel.Info, "Performance");
}
else if (loadTime < 5000)
{
    LogWithLevel($"?? Acceptable performance: {loadTime}ms", LogLevel.Warning, "Performance");
}
else
{
    LogWithLevel($"? Poor performance: {loadTime}ms", LogLevel.Error, "Performance");
}
```

## ReportPortal Configuration

### Setting Up Log Level Filtering
1. Open ReportPortal dashboard
2. Navigate to your test execution
3. Use the log level filter dropdown
4. Select desired levels:
   - **All**: TRACE, DEBUG, INFO, WARNING, ERROR, FATAL
   - **Production**: INFO, WARNING, ERROR, FATAL
   - **Errors Only**: ERROR, FATAL
   - **Health Check**: WARNING, ERROR, FATAL

### Best Practices

#### 1. Consistent Level Usage
- Always use the same level for similar types of messages
- Establish team conventions for log level usage

#### 2. Category Tagging
```csharp
LogWithLevel("Message", LogLevel.Info, "Specific Category");
```

#### 3. Progressive Disclosure
- Use TRACE for detailed flow
- Use DEBUG for technical details
- Use INFO for important actions
- Use WARNING for issues
- Use ERROR for failures
- Use FATAL for critical problems

#### 4. Filtering Strategy
- **Development**: Use all levels
- **CI/CD**: Filter to INFO and above
- **Production**: Filter to WARNING and above
- **Incident Response**: Filter to ERROR and FATAL

## Log Level Matrix

| Scenario | TRACE | DEBUG | INFO | WARN | ERROR | FATAL |
|----------|-------|-------|------|------|-------|-------|
| Development | ? | ? | ? | ? | ? | ? |
| CI/CD | ? | ? | ? | ? | ? | ? |
| Production | ? | ? | ? | ? | ? | ? |
| Health Check | ? | ? | ? | ? | ? | ? |
| Error Analysis | ? | ? | ? | ? | ? | ? |
| Critical Only | ? | ? | ? | ? | ? | ? |

## Implementation in Tests

### Example Test with Full Log Level Usage
```csharp
[Test]
public async Task ExampleWithAllLogLevels()
{
    // INFO: Test start
    LogWithLevel("?? Starting comprehensive test", LogLevel.Info, "Test Start");
    
    // TRACE: Detailed flow
    LogWithLevel("Entering test execution phase", LogLevel.Trace, "Flow");
    
    // DEBUG: Technical details
    LogWithLevel("Browser instance initialized", LogLevel.Debug, "Technical");
    
    // INFO: Main actions
    LogWithLevel("Navigating to target page", LogLevel.Info, "Navigation");
    
    // WARNING: Non-critical issues
    LogWithLevel("?? Optional feature not available", LogLevel.Warning, "Feature");
    
    // ERROR: Test failures
    LogWithLevel("? Assertion failed", LogLevel.Error, "Assertion");
    
    // FATAL: Critical failures
    LogWithLevel("?? System crash detected", LogLevel.Fatal, "System");
}
```

This comprehensive log level system enables powerful filtering and analysis capabilities in ReportPortal, allowing teams to focus on relevant information based on their current needs and context.