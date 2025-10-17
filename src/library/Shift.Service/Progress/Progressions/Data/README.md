# Progress / Progressions / Data

Progressions is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Progressions.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QEnrollment` from schema `records` to schema `progress`.
* Rename table from `QEnrollment` to `QGradebookEnrollment`.
* Move table `QEnrollmentHistory` from schema `records` to schema `progress`.
* Rename table from `QEnrollmentHistory` to `QEnrollmentChange`.
* Move table `QGradebookCompetencyValidation` from schema `records` to schema `progress`.
* Rename table from `QGradebookCompetencyValidation` to `QProgressionValidation`.
* Move table `QLearnerProgramSummary` from schema `reports` to schema `progress`.
* Move table `QProgress` from schema `records` to schema `progress`.
* Rename table from `QProgress` to `QProgression`.
* Move table `QProgressHistory` from schema `records` to schema `progress`.
* Rename table from `QProgressHistory` to `QProgressionChange`.
