# Shift.Test

This project contains unit tests suitable for including in the rapid test automation pipeline.

Tests in this library target .NET 9.


## Reminders

This project is not yet ready for automation. The unit tests in the Integration component need to be
improved so the current environment is determined from appsettings.


## Naming conventions

The naming convention for unit tests is `ClassTests.Method_Scenario_ExpectedResult`.

Each unit test method should follow the same basic pattern:

<code>
[Fact]
public void Method_Scenario_Result()
{
    // 1. Arrange
    // 2. Act
    // 3. Assert
}
</code>