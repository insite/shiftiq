# Variant / CMDS / Data

CMDS is part of the Variant subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Variant CMDS.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QCmdsInvoice` from schema `custom_cmds` to schema `variant`.
* Move table `QCmdsInvoiceFee` from schema `custom_cmds` to schema `variant`.
* Move table `QCmdsInvoiceItem` from schema `custom_cmds` to schema `variant`.
* Move table `QUserStatus` from schema `custom_cmds` to schema `variant`.
* Rename table from `QUserStatus` to `QLearnerSummary`.
* Move table `TCollegeCertificate` from schema `custom_cmds` to schema `variant`.
* Move table `TUserStatus` from schema `custom_cmds` to schema `variant`.
* Rename table from `TUserStatus` to `TLearnerMeasurement`.
* Move table `ZUserStatus` from schema `custom_cmds` to schema `variant`.
* Rename table from `ZUserStatus` to `TLearnerSnapshot`.
* Move table `ZUserStatusSummary` from schema `custom_cmds` to schema `variant`.
* Rename table from `ZUserStatusSummary` to `TLearnerSnapshotSummary`.
