# Progress / Credentials / Data

Credentials is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Credentials.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `ContactExperience` from schema `contacts` to schema `progress`.
* Rename table from `ContactExperience` to `TLearnerExperience`.
* Move table `QCredential` from schema `achievements` to schema `progress`.
* Move table `QCredentialHistory` from schema `records` to schema `progress`.
* Rename table from `QCredentialHistory` to `QCredentialChange`.
