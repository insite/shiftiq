# Assessment / Questions / Data

Questions is part of the Assessment subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Assessment Questions.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QBank` from schema `banks` to schema `assessment`.
* Move table `QBankForm` from schema `banks` to schema `assessment`.
* Move table `QBankOption` from schema `banks` to schema `assessment`.
* Move table `QBankQuestion` from schema `banks` to schema `assessment`.
* Move table `QBankQuestionAttachment` from schema `banks` to schema `assessment`.
* Move table `QBankQuestionGradeItem` from schema `banks` to schema `assessment`.
* Rename table from `QBankQuestionGradeItem` to `QBankFormQuestionGradeitem`.
* Move table `QBankQuestionSubCompetency` from schema `banks` to schema `assessment`.
* Rename table from `QBankQuestionSubCompetency` to `QBankQuestionCompetency`.
* Move table `QBankSpecification` from schema `banks` to schema `assessment`.
* Move table `TQuiz` from schema `assessments` to schema `assessment`.
