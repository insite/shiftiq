# Directory / People / Data

People is part of the Directory subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Directory People.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QPerson` from schema `contacts` to schema `directory`.
* Move table `QPersonAddress` from schema `contacts` to schema `directory`.
* Move table `QPersonSecret` from schema `contacts` to schema `directory`.
* Move table `TPersonField` from schema `contacts` to schema `directory`.
