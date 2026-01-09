# Engine.Api

Engine.Api is a .NET 9 API that implements a simple microservice wrapper for the Google Translate API.

It maintains a persistent database cache for every translation, which minimizes the number of translation requests submitted to Google. This improves performance and minimizes usage cost.

The scope for this code is limited to kernel functionality. It is general-purpose code that has no dependencies (direct or indirect) on any specific business application context.

## Proposed Improvements

In the future, when time and opportunity permit, the following improvements should be considered:

### Rename Project

Rename the project to `Shift.Kernel.Integration.Google` so the project name follows current coding conventions.

### Integrate Location API

Integrate with the Google Location API. Currently, the country and province tables are static and the data is not reviewed or maintained. Also, many more location-related features can be made available to clients of this API by utilizing the Google Location API.