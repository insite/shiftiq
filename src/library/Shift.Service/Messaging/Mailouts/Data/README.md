# Messaging / Mailouts / Data

Mailouts is part of the Messaging subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Messaging Mailouts.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QCarbonCopy` from schema `communications` to schema `messaging`.
* Rename table from `QCarbonCopy` to `QMailoutRecipientCopy`.
* Move table `QClick` from schema `messages` to schema `messaging`.
* Rename table from `QClick` to `QMailoutClick`.
* Move table `QMailout` from schema `communications` to schema `messaging`.
* Move table `QRecipient` from schema `communications` to schema `messaging`.
* Rename table from `QRecipient` to `QMailoutRecipient`.
