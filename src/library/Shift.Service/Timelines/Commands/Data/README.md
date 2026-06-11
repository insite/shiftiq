# Timeline / Commands / Data

Commands is part of the Timeline subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Timeline Commands.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `Command` from schema `logs` to schema `timeline`.
* Rename table from `Command` to `TCommand`.
