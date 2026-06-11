# Learning / Programs / Data

Programs is part of the Learning subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Learning Programs.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TProgram` from schema `records` to schema `learning`.
* Move table `TTask` from schema `records` to schema `learning`.
