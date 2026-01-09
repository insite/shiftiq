# Engine.CssToHtml

Engine.CssToHtml is a .NET 9 API that implements a simple microservice wrapper for the [PreMailer.Net](https://github.com/milkshakesoftware/PreMailer.Net) library.

PreMailer.NET 2.6 has a tightly coupled dependency on AngleSharp 1.1. If AngleSharp is upgraded to a new version, then this causes PreMailer to throw exceptions. All references to the PreMailer assembly have been removed from all other projects so that AngleSharp can be updated in other projects without breaking functionality that requires PreMailer.

## Proposed Improvements

In the future, when time and opportunity permit, the following improvements should be considered:

### Rename Project

Rename the project to `Shift.Kernel.Integration.PreMailer` so the project name follows current coding conventions.