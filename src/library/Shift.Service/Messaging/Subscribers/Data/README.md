# Messaging / Subscribers / Data

Subscribers is part of the Messaging subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Messaging Subscribers.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QFollower` from schema `messages` to schema `messaging`.
* Rename table from `QFollower` to `QSubscriberFollower`.
* Move table `QSubscriberGroup` from schema `messages` to schema `messaging`.
* Move table `QSubscriberUser` from schema `messages` to schema `messaging`.
