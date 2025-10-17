# Progress / Achievements / Data

Achievements is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Achievements.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QAchievement` from schema `achievements` to schema `progress`.
* Move table `QAchievementPrerequisite` from schema `records` to schema `progress`.
* Move table `TAchievementCategory` from schema `record` to schema `progress`.
* Move table `TAchievementDepartment` from schema `achievements` to schema `progress`.
* Rename table from `TAchievementDepartment` to `TAchievementGroup`.
* Move table `TAchievementOrganization` from schema `achievements` to schema `progress`.
