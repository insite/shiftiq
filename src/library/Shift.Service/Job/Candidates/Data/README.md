# Job / Candidates / Data

Candidates is part of the Job subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Job Candidates.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TCandidateEducation` from schema `jobs` to schema `job`.
* Move table `TCandidateExperience` from schema `jobs` to schema `job`.
* Move table `TCandidateExperienceItem` from schema `jobs` to schema `job`.
* Move table `TCandidateLanguageProficiency` from schema `jobs` to schema `job`.
* Move table `TCandidateUpload` from schema `jobs` to schema `job`.
* Rename table from `TCandidateUpload` to `TCandidateFile`.
