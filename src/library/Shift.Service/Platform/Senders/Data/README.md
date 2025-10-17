# Platform / Senders / Data

Senders is part of the Platform subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Platform Senders.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TSender` from schema `accounts` to schema `platform`.
* Move table `TSenderOrganization` from schema `accounts` to schema `platform`.
