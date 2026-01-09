# Progress / Gradebooks / Data

Gradebooks is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Gradebooks.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QGradebook` from schema `records` to schema `progress`.
* Move table `QGradebookEvent` from schema `records` to schema `progress`.
* Move table `QGradeItem` from schema `records` to schema `progress`.
* Rename table from `QGradeItem` to `QGradeitem`.
* Move table `QGradeItemCompetency` from schema `records` to schema `progress`.
* Rename table from `QGradeItemCompetency` to `QGradeitemCompetency`.
