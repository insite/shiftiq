# Security / Permissions / Data

Permissions is part of the Security subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Security Permissions.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TGroupPermission` from schema `contacts` to schema `security`.
* Rename table from `TGroupPermission` to `TPermission`.
