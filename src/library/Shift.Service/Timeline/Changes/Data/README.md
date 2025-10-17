# Timeline / Changes / Data

Changes is part of the Timeline subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Timeline Changes.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `Aggregate` from schema `logs` to schema `timeline`.
* Rename table from `Aggregate` to `TAggregate`.
* Move table `Change` from schema `logs` to schema `timeline`.
* Rename table from `Change` to `TChange`.
* Move table `Snapshot` from schema `logs` to schema `timeline`.
* Rename table from `Snapshot` to `TSnapshot`.
