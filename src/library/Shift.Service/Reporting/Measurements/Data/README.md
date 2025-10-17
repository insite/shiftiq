# Reporting / Measurements / Data

Measurements is part of the Reporting subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Reporting Measurements.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `TMeasurement` from schema `reports` to schema `reporting`.
