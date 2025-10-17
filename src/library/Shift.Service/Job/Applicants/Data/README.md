# Job / Applicants / Data

Applicants is part of the Job subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Job Applicants.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TApplication` from schema `jobs` to schema `job`.
* Rename table from `TApplication` to `TApplicant`.
