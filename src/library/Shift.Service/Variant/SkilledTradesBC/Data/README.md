# Variant / SkilledTradesBC / Data

SkilledTradesBC is part of the Variant subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Variant SkilledTradesBC.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `ExamDistributionRequest` from schema `custom_ita` to schema `variant`.
* Rename table from `ExamDistributionRequest` to `TDistribution`.
* Move table `Individual` from schema `custom_ita` to schema `variant`.
* Rename table from `Individual` to `TIndividual`.
