# Progress / Logbooks / Data

Logbooks is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Logbooks.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QExperience` from schema `records` to schema `progress`.
* Rename table from `QExperience` to `QLogbookExperience`.
* Move table `QExperienceCompetency` from schema `records` to schema `progress`.
* Rename table from `QExperienceCompetency` to `QLogbookCompetency`.
* Move table `QJournal` from schema `records` to schema `progress`.
* Rename table from `QJournal` to `QLogbook`.
* Move table `QJournalSetupUser` from schema `records` to schema `progress`.
* Rename table from `QJournalSetupUser` to `QRegulationUser`.
