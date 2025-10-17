# Record / Achievements / Data

Achievements is a feature set in the Record application component.
  
Classes in this Data folder implement data access for this feature set. This is the persistence layer for Achievements.

## (Proposed) Future Schema Changes

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current database naming conventions:

* Move table `QAchievement` from schema `achievements` to schema `record`.
* Move table `QAchievementPrerequisite` from schema `records` to schema `record`.
* Move table `TAchievementDepartment` from schema `achievements` to schema `record`.
* Rename table from `TAchievementDepartment` to `TAchievementGroup`.
* Move table `TAchievementOrganization` from schema `achievements` to schema `record`.
