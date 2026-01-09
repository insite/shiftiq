# Integration / Moodle / Data

Moodle is part of the Integration subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Integration Moodle.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Rename table from `TMoodleEvent` to `TMoodleChange`.
