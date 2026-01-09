# Engine.Scorm

Engine.Scorm is a .NET 9 API that implements a simple microservice wrapper for the SCORM Cloud API.

The SCORM Cloud SDK (Com.RusticiSoftware.Cloud.V2 4.0) has a tightly coupled dependency on RestSharp 109.0. If RestSharp is upgraded to a new version, then this causes the SCORM Cloud SDK to throw exceptions. All references to the SCORM Cloud SDK assembly have been removed from all other projects so that RestSharp can be updated in all other projects without breaking functionality that requires SCORM.

Please note the RestSharp 109.0 package has at least one vulnerability with moderate severity, therefore it has been removed from all other projects also. It is permitted here because it is isolated and encapsulated (and required by the SCORM Cloud SDK).

## Proposed Improvements

In the future, when time and opportunity permit, the following improvements should be considered:

### Rename Project

Rename the project to `Shift.Kernel.Integration.ScormCloud` so the project name follows current coding conventions.