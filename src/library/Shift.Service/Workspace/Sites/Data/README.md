# Workspace / Sites / Data

Sites is part of the Workspace subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Workspace Sites.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QSite` from schema `sites` to schema `workspace`.
