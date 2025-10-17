# Assessment / Rubrics / Data

Rubrics is part of the Assessment subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Assessment Rubrics.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QRubric` from schema `records` to schema `assessment`.
* Move table `QRubricCriterion` from schema `records` to schema `assessment`.
* Move table `QRubricRating` from schema `records` to schema `assessment`.
