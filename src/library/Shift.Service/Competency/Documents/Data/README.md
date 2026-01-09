# Competency / Documents / Data

Documents is part of the Competency subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Competency Documents.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QDocument` from schema `standards` to schema `competency`.
* Move table `QDocumentCompetency` from schema `standards` to schema `competency`.
* Move table `QRelatedDocument` from schema `standards` to schema `competency`.
* Rename table from `QRelatedDocument` to `QDocumentConnection`.
