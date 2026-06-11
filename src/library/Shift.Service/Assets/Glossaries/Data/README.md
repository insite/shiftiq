# Content / Glossaries / Data

Glossaries is part of the Content subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Content Glossaries.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QGlossaryTerm` from schema `assets` to schema `content`.
* Move table `QGlossaryTermContent` from schema `assets` to schema `content`.
* Rename table from `QGlossaryTermContent` to `QGlossaryContent`.
