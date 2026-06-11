# Assessment / Answers / Data

Answers is part of the Assessment subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Assessment Answers.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QAttempt` from schema `assessments` to schema `assessment`.
* Move table `QAttemptMatch` from schema `assessments` to schema `assessment`.
* Move table `QAttemptOption` from schema `assessments` to schema `assessment`.
* Move table `QAttemptPin` from schema `assessments` to schema `assessment`.
* Move table `QAttemptQuestion` from schema `assessments` to schema `assessment`.
* Move table `QAttemptSection` from schema `assessments` to schema `assessment`.
* Move table `QAttemptSolution` from schema `assessments` to schema `assessment`.
* Move table `TLearnerAttemptSummary` from schema `assessments` to schema `assessment`.
* Rename table from `TLearnerAttemptSummary` to `TLearnerFormAttempt`.
* Move table `TQuizAttempt` from schema `assessments` to schema `assessment`.
