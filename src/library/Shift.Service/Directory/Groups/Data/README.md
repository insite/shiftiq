# Directory / Groups / Data

Groups is part of the Directory subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Directory Groups.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QGroup` from schema `contacts` to schema `directory`.
* Move table `QGroupAddress` from schema `contacts` to schema `directory`.
* Move table `QGroupConnection` from schema `contacts` to schema `directory`.
* Move table `QGroupTag` from schema `contacts` to schema `directory`.
* Move table `TGroupSetting` from schema `contacts` to schema `directory`.
* Rename table from `TGroupSetting` to `TGroupField`.
