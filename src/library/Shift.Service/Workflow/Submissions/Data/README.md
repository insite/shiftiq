# Workflow / Submissions / Data

Submissions is part of the Workflow subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Workflow Submissions.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QResponseAnswer` from schema `surveys` to schema `workflow`.
* Move table `QResponseOption` from schema `surveys` to schema `workflow`.
* Move table `QResponseSession` from schema `surveys` to schema `workflow`.
* Rename table from `QResponseSession` to `QResponse`.
