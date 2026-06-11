# Progress / Certificates / Data

Certificates is part of the Progress subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Progress Certificates.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TCertificateLayout` from schema `records` to schema `progress`.
* Rename table from `TCertificateLayout` to `TCertificate`.
