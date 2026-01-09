# Platform / Partitions / Data

Partitions is part of the Platform subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Platform Partitions.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TPartitionSetting` from schema `security` to schema `platform`.
* Rename table from `TPartitionSetting` to `TPartitionField`.
