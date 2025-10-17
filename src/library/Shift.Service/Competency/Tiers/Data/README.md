# Competency / Tiers / Data

Tiers is part of the Competency subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Competency Tiers.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QStandardTier` from schema `standard` to schema `competency`.
