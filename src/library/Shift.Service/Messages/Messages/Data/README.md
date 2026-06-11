# Messaging / Messages / Data

Messages is part of the Messaging subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Messaging Messages.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QLink` from schema `messages` to schema `messaging`.
* Rename table from `QLink` to `QMessageLink`.
* Move table `QMessage` from schema `messages` to schema `messaging`.
