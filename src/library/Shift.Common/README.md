# Shift.Common

The Common (or Base) library implements general-purpose functionality with no dependencies on any specific application, organization, or presentation layer.

It targets .NET Standard 2.0 for maximum reusability, including .NET (Core) and .NET Framework applications.

It contains only a bare minimum set of dependencies on third-party libraries:

- **Markdig** for Markdown-to-HTML transformation
- **Newtonsoft** for JSON serialization
- **Serilog** for structured logging

## Proposed Improvements

In the future, when time and opportunity permit, the following improvements should be considered:

### Rename Project

Rename the project to `Shift.Kernel` so the name aligns more closely with its role in the overall system architecture.