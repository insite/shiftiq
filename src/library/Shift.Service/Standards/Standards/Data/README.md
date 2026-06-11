# Competency / Standards / Data

Standards is part of the Competency subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Competency Standards.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `DepartmentProfileCompetency` from schema `standards` to schema `competency`.
* Rename table from `DepartmentProfileCompetency` to `TProfileGroupCompetency`.
* Move table `DepartmentProfileUser` from schema `standards` to schema `competency`.
* Rename table from `DepartmentProfileUser` to `TProfileGroupLearner`.
* Move table `QStandard` from schema `standard` to schema `competency`.
* Move table `QStandardAchievement` from schema `standard` to schema `competency`.
* Move table `QStandardCategory` from schema `standard` to schema `competency`.
* Move table `QStandardConnection` from schema `standard` to schema `competency`.
* Move table `QStandardContainment` from schema `standard` to schema `competency`.
* Move table `QStandardGroup` from schema `standard` to schema `competency`.
* Move table `QStandardOrganization` from schema `standard` to schema `competency`.
