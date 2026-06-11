# Booking / Registrations / Data

Registrations is part of the Booking subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Booking Registrations.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QAccommodation` from schema `registrations` to schema `booking`.
* Rename table from `QAccommodation` to `QRegistrationAccommodation`.
* Move table `QRegistration` from schema `registrations` to schema `booking`.
* Move table `QRegistrationInstructor` from schema `registrations` to schema `booking`.
* Move table `QRegistrationTimer` from schema `registrations` to schema `booking`.
