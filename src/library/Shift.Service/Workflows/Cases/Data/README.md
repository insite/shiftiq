# Workflow / Cases / Data

Cases is part of the Workflow subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Workflow Cases.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QIssue` from schema `issues` to schema `workflow`.
* Rename table from `QIssue` to `QCase`.
* Move table `QIssueAttachment` from schema `issues` to schema `workflow`.
* Rename table from `QIssueAttachment` to `QCaseFile`.
* Move table `QIssueFileRequirement` from schema `issues` to schema `workflow`.
* Rename table from `QIssueFileRequirement` to `QCaseFileRequirement`.
* Move table `QIssueGroup` from schema `issues` to schema `workflow`.
* Rename table from `QIssueGroup` to `QCaseGroup`.
* Move table `QIssueUser` from schema `issues` to schema `workflow`.
* Rename table from `QIssueUser` to `QCaseUser`.
* Move table `TIssueStatus` from schema `issues` to schema `workflow`.
* Rename table from `TIssueStatus` to `TCaseStatus`.
