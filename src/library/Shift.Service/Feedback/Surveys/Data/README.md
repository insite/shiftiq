# Feedback / Surveys / Data

Surveys is part of the Feedback subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Feedback Surveys.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QSurveyCondition` from schema `surveys` to schema `feedback`.
* Move table `QSurveyForm` from schema `surveys` to schema `feedback`.
* Rename table from `QSurveyForm` to `QSurvey`.
* Move table `QSurveyOptionItem` from schema `surveys` to schema `feedback`.
* Move table `QSurveyOptionList` from schema `surveys` to schema `feedback`.
* Move table `QSurveyQuestion` from schema `surveys` to schema `feedback`.
