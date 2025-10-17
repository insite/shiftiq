# Booking / Events / Data

Events is part of the Booking subsystem.
  
The **Data** folder contains code for - including entities, entity type configurations, entity readers and readers, entity adapters, and entity services. This is the **persistence** (or entity) layer for Booking Events.

## Proposed Improvements

When time and opportunity permit, the following database schema changes should be considered, to improve alignment with current naming conventions:

* Move table `QEvent` from schema `events` to schema `booking`.
* Move table `QEventAssessmentForm` from schema `events` to schema `booking`.
* Rename table from `QEventAssessmentForm` to `QEventForm`.
* Move table `QEventAttendee` from schema `events` to schema `booking`.
* Rename table from `QEventAttendee` to `QEventUser`.
* Move table `QEventTimer` from schema `events` to schema `booking`.
* Move table `QSeat` from schema `events` to schema `booking`.
* Rename table from `QSeat` to `QEventSeat`.
