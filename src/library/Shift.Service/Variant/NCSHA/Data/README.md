# Variant / NCSHA / Data

NCSHA is part of the Variant subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Variant NCSHA.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `AbProgram` from schema `custom_ncsha` to schema `variant`.
* Rename table from `AbProgram` to `TSurveyAB`.
* Move table `Counter` from schema `custom_ncsha` to schema `variant`.
* Rename table from `Counter` to `TCounter`.
* Move table `Field` from schema `custom_ncsha` to schema `variant`.
* Rename table from `Field` to `TReportField`.
* Move table `Filter` from schema `custom_ncsha` to schema `variant`.
* Rename table from `Filter` to `TReportFilter`.
* Move table `HcProgram` from schema `custom_ncsha` to schema `variant`.
* Rename table from `HcProgram` to `TSurveyHC`.
* Move table `HiProgram` from schema `custom_ncsha` to schema `variant`.
* Rename table from `HiProgram` to `TSurveyHI`.
* Move table `History` from schema `custom_ncsha` to schema `variant`.
* Rename table from `History` to `TReportChange`.
* Move table `MfProgram` from schema `custom_ncsha` to schema `variant`.
* Rename table from `MfProgram` to `TSurveyMF`.
* Move table `MrProgram` from schema `custom_ncsha` to schema `variant`.
* Rename table from `MrProgram` to `TSurveyMR`.
* Move table `PaProgram` from schema `custom_ncsha` to schema `variant`.
* Rename table from `PaProgram` to `TSurveyPA`.
* Move table `ProgramFolder` from schema `custom_ncsha` to schema `variant`.
* Rename table from `ProgramFolder` to `TReportFolder`.
* Move table `ProgramFolderComment` from schema `custom_ncsha` to schema `variant`.
* Rename table from `ProgramFolderComment` to `TReportFolderComment`.
* Move table `TReportMapping` from schema `custom_ncsha` to schema `variant`.
