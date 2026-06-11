# Billing / Invoices / Data

Invoices is part of the Billing subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Billing Invoices.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QInvoice` from schema `invoices` to schema `billing`.
* Move table `QInvoiceItem` from schema `invoices` to schema `billing`.
