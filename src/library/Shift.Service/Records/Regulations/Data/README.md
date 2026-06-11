# Progress / Regulations / Data

Regulations is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Regulations.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QCompetencyRequirement` from schema `records` to schema `progress`.
* Rename table from `QCompetencyRequirement` to `QRegulationCompetency`.
* Move table `QJournalSetup` from schema `records` to schema `progress`.
* Rename table from `QJournalSetup` to `QRegulation`.
* Move table `QJournalSetupField` from schema `records` to schema `progress`.
* Rename table from `QJournalSetupField` to `QRegulationField`.
* Move table `QJournalSetupGroup` from schema `records` to schema `progress`.
* Rename table from `QJournalSetupGroup` to `QRegulationGroup`.
