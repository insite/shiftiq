# Metadata / Upgrades / Data

Upgrades is part of the Metadata subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Metadata Upgrades.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `Upgrade` from schema `utilities` to schema `metadata`.
* Rename table from `Upgrade` to `TUpgrade`.
