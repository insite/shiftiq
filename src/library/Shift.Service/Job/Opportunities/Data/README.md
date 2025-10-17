# Job / Opportunities / Data

Opportunities is part of the Job subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Job Opportunities.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TOpportunity` from schema `jobs` to schema `job`.
* Move table `TOpportunityCategory` from schema `jobs` to schema `job`.
