# Security / Users / Data

Users is part of the Security subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Security Users.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QUser` from schema `identities` to schema `security`.
* Move table `QUserConnection` from schema `identities` to schema `security`.
* Move table `TUserAuthenticationFactor` from schema `accounts` to schema `security`.
* Rename table from `TUserAuthenticationFactor` to `TUserToken`.
* Move table `TUserComment` from schema `accounts` to schema `security`.
* Move table `TUserMock` from schema `accounts` to schema `security`.
* Move table `TUserSession` from schema `identity` to schema `security`.
* Move table `TUserSessionCache` from schema `accounts` to schema `security`.
* Move table `TUserSetting` from schema `accounts` to schema `security`.
* Rename table from `TUserSetting` to `TUserField`.
