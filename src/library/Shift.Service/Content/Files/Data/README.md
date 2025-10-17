# Content / Files / Data

Files is part of the Content subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Content Files.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TFile` from schema `assets` to schema `content`.
* Move table `TFileActivity` from schema `assets` to schema `content`.
* Move table `TFileClaim` from schema `assets` to schema `content`.
* Move table `Upload` from schema `resources` to schema `content`.
* Rename table from `Upload` to `TUpload`.
* Move table `UploadRelation` from schema `resources` to schema `content`.
* Rename table from `UploadRelation` to `TUploadObject`.
