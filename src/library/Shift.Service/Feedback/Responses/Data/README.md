# Feedback / Responses / Data

Responses is part of the Feedback subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Feedback Responses.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QResponseAnswer` from schema `surveys` to schema `feedback`.
* Move table `QResponseOption` from schema `surveys` to schema `feedback`.
* Move table `QResponseSession` from schema `surveys` to schema `feedback`.
* Rename table from `QResponseSession` to `QResponse`.
