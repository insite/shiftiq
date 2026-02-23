# Shift API

Here is the list of subsystems in the Shift API:

* **Domain Subsystems (Application Features)**
  * Billing (Sales)
  * Booking (Calendar/Event/Schedule)
  * Competency (Standard)
  * Content (Assets/Files)
  * Directory (Contact)
  * Evaluation (Assessment)
  * Learning (Course)
  * Messaging (Message)
  * Progress (Record)
  * Reporting
  * Workflow (Survey/Form)
  * Workspace (Site/Page)
  
* **Plugin Subsystems**
  * Integration
  * Variant

* **Utility Subsystems**
  * Diagnostic
  * Internal
  * Lab 
  * Metadata
  * Security
  * Setup
  * Timeline

* **Shell Subsystems**
  * Lobby
  * Me
  * Portal


## Domains

**Domains** are the core business subsystems. They implement the primary features of the system. Each domain represents a distinct bounded context with its own data ownership, business rules, and API surface. Domains are designed to be cohesive and loosely coupled, exposing well-defined interfaces that other subsystems can depend on without requiring a lot of knowledge about internal implementation details. They form the functional backbone of the system and directly support the system features that customers use.

- **Billing** manages financial transactions, invoices, payments, and subscription lifecycles. It handles pricing tiers, discount applications, and recurring charges.

- **Booking** manages calendar events, scheduling, and registrations. It supports event creation, attendee management, instructor assignments, and accommodation tracking.

- **Competency** defines and manages skills, capabilities, and proficiency standards. It supports alignment with roles, training plans, learning paths, and assessments.

- **Content** manages files, media assets, and translations. It handles file storage, upload/download operations, and multilingual content support.

- **Directory** manages people, groups, and organizational structures. It models relationships such as reporting lines and group memberships across the platform.

- **Evaluation** manages assessments, quizzes, and question banks. It handles question authoring, test delivery, attempt tracking, and answer grading.

- **Learning** manages courses, learning paths, and educational activities. It supports enrollments, unit sequencing, and integration with competencies and assessments.

- **Messaging** manages email, SMS, and notification delivery. It handles message templates, mailouts, delivery tracking, and communication scheduling.

- **Progress** tracks learner advancement, achievements, and completion states. It manages gradebooks, training and education credentials, and time-based progression metrics.

- **Reporting** provides data extraction and analytics capabilities. It supports generating reports, dashboards, and data exports across domain subsystems.

- **Workflow** manages forms, surveys, and case management processes. It handles form submissions, conditional logic, and workflow-driven approvals.

- **Workspace** manages sites, pages, and navigation structures. It defines how features are grouped and accessed across roles and organizations.

## Plugins

**Plugins** are subsystems that extend or adapt system features without modifying core domain logic. They provide extensibility points for third-party integrations, tenant-specific variations, and optional features that may not apply to all deployments. Plugins depend on domains but domains do not depend on plugins, preserving a clean dependency direction and allowing the system to evolve without destabilizing its foundation.

- **Integration** provides connectivity with third-party systems and external services. It encapsulates adapters, synchronization logic, and data exchange protocols to enable interoperability without coupling domain subsystems to external dependencies.

- **Variant** supports custom features that are specific to individual partitions, tenants, or organizations. It isolates bespoke functionality from the shared system codebase, allowing tenant-specific behavior without fragmenting the core system.

## Utilities

**Utilities** are infrastructure subsystems that provide cross-cutting operational features to support overall system functionality and maintenance. They are not tied to specific business features but instead offer foundational services such as security, configuration, diagnostics, and metadata management. Utilities may be consumed by domains, plugins, and shells alike, and do not contain business logic or own domain-specific data.

- **Diagnostic** exposes health check and version endpoints for monitoring and operational visibility. It provides lightweight endpoints that infrastructure tools use to verify system status.

- **Internal** is a special-purpose utility subsystem that encapsulates non-public features used exclusively for system maintenance, diagnostics, and testing. It contains tools, endpoints, and workflows that support internal development, staging, and operations. It is not intended for tenant access or external API consumers. The Internal subsystem provides isolated access to infrastructure-level features without exposing them in the public API surface. It should be treated as privileged and access-restricted in all environments.

- **Lab** provides a sandbox environment for experimental features and prototyping. It isolates work-in-progress features from production-ready subsystems.

- **Metadata** offers utilities for inspection, parsing, and validation of entity specifications, database schemas, and data formats. It supports debugging and troubleshooting by providing a variety of introspection tools.

- **Security** manages authentication, authorization, and identity across the system. It handles user accounts, organization accounts, roles, permissions, API tokens, secrets, and session management.

- **Setup** exposes endpoints for API route discovery, permission mapping, and OpenAPI specification access. It supports developer tooling and operational configuration of the system.

- **Timeline** provides a command and query interface for the event sourcing infrastructure. It enables audit trails, change tracking, and event-driven workflows by relaying commands to the event store and executing queries against projected state.

## Shells

**Shells** are responsible for composition of front-end and back-end features from multiple subsystems to deliver unified, session-aware views for client applications. They act as a bridge between domain-specific subsystems and the user interface, assembling contextual models such as user session state, dashboard metadata, and cross-subsystem summaries. Shells are not responsible for business logic or data ownership, but instead coordinate and shape data from underlying services to support runtime experience delivery, particularly for dynamic front-end rendering and role-based navigation.

- **Lobby** provides the initial entry experience for users before they authenticate or select their organization. It handles pre-session rendering, organization discovery, and multi-tenant (i.e., multi-organization and/or multi-partition) landing experiences.

- **Me** aggregates user identity, permissions, environment settings, and UI configuration from multiple subsystems into a unified session context. The resulting model supports front-end application initialization and role-based navigation for the currently authenticated user.

- **Portal** assembles partition-specific, organization-specific, and role-specific dashboard views by coordinating data from multiple domain subsystems. It provides cross-subsystem summaries and navigation structures tailored to the user's organizational context.


## API subsystem aliases for UI toolkits


### Rationale for Billing instead of Sales

The Billing API subsystem manages everything within the Sales UI toolkit. The name Billing better reflects its functional purpose, which improves clarity for developers and product stakeholders.

**Sales** is too generic and business-focused, easily confused with:
* Sales team activities and CRM features
* Lead generation and prospect management
* Marketing campaigns and conversion tracking
* Business intelligence and sales analytics

Additionally, it creates misleading API routes that don't accurately represent technical functionality:

```
GET /sales/invoices
GET /sales/payments
```

**Billing** clearly communicates the core technical intent of this subsystem:

* Processing payments and transactions
* Managing invoices, receipts, and financial records
* Handling subscription lifecycles and recurring charges
* Managing pricing tiers and discount applications

This name aligns with financial system conventions and improves API discoverability. For example:

```
GET /billing/invoices/{id}
GET /billing/payments/{id}
GET /billing/subscriptions/{id}
```

The subsystem focuses on the technical implementation of financial transactions rather than the broader business process of selling, and this makes "Billing" a more accurate and descriptive term for its actual responsibilities within the platform architecture.


### Rationale for Booking instead of Events

The Booking API subsystem manages everything within the Events UI toolkit. The name Booking better reflects its true functional purpose and improves clarity for developers and product stakeholders.

**Events** is too generic and ambiguous, easily confused with:

* System-generated audit events (e.g., Timeline changes)
* Domain events in event-driven architectures
* Calendar appointments or notifications
* The event keyword in C#

Additionally, it leads to awkward and repetitive API routes such as:

```
GET /events/events
```

**Booking** clearly communicates the core intent of this subsystem:

* Scheduling calendar events
* Managing reservations, availabilities, enrollments

This name aligns with RESTful conventions and improves API discoverability. For example:

```
GET /booking/events/{id}
GET /booking/registrations/{id}
```


### Rationale for Competency instead of Standards

The Competency API subsystem manages everything within the Standards UI toolkit. The name Competency better represents its functional purpose and enhance clarity for developers, domain experts, and product stakeholders.

**Standards** is too broad and ambiguous, easily confused with:

* Technical specifications
* System-wide configuration guidelines, best practices, or common conventions
* Default values

Additionally, it leads to awkward and repetitive API routes such as:

```
GET /standards/standards
```

**Competency** more accurately conveys the intent for this subsystem:

* Defining and managing skills, capabilities, and proficiencies
* Organizing learning objectives and assessment criteria
* Supporting alignment with roles, training plans, learning paths, and assessments

This name aligns with widely accepted terminology in learning, HR, and talent platforms, and improves API clarity and discoverability. For example:

```
GET /competency/areas
GET /competency/documents
GET /competency/frameworks
GET /competency/levels/{id}
GET /competency/profiles
```

### Rationale for Directory instead of Contacts

The Directory API subsystem manages everything within the Contacts UI toolkit. The name Directory more accurately reflects its scope and improves semantic clarity for developers and product stakeholders.

**Contacts** is overly narrow and potentially misleading, because it can be easily misinterpreted (by a newcomer) as:

* A basic address book or personal contact list
* A CRM-style record for outreach or correspondence
* A storage of static contact details

**Directory** better communicates the purpose of the subsystem:

* Managing people, groups, and organizational substructures
* Modeling relationships (e.g., reporting lines, memberships)
* Supporting identity-aware functionality across the platform

This name also aligns with enterprise and identity system conventions (e.g., Active Directory, Microsoft Graph) and improves API clarity and consistency. For example:

```
GET /directory/groups/{id}
GET /directory/people/{id}
```


### Rationale for Learning instead of Courses

The Learning API subsystem manages everything within the Courses UI toolkit. The name Learning better reflects the broader scope of functionality and improves clarity and extensibility for developers and product stakeholders.

**Courses** was too narrow and prescriptive, easily misinterpreted as:

* A single unit of structured training content
* A rigid, classroom-style experience
* Limited to traditional course catalogs or enrollments

The name Courses has become increasingly inaccurate as the subsystem evolved to support a wide range of educational experiences. Additionally, it leads to awkward and repetitive API routes such as:

```
GET /courses/courses
```

**Learning** better reflects the purpose and capabilities of the subsystem:

* Modular learning paths
* Integrating with competencies, assessments, and feedback
* Blended and adaptive learning models

This name also aligns with clear and very readable conventions. For example:

```
GET /learning/courses/{id}
GET /learning/courses/{id}/enrollments
GET /learning/courses/{id}/units
GET /learning/activities/{id}
```


### Rationale for Messaging instead of Messages

The Messaging API subsystem manages everything within the Messages UI toolkit. The name Messaging more accurately represents the subsystem's purpose and improves clarity for developers and administrators.

**Messages** is too object-oriented in nature, and it is easily misinterpreted as:

* Individual email message records
* Isolated notifications or alerts
* A narrow model focused on individual message payloads

This leads to semantic confusion when the subsystem is actually responsible for broader communication workflows, such as:

* Sending and managing email messages and SMS messages
* Supporting systems notifications and alerts
* Coordinating delivery status, batching, scheduling, and retries

> In the future, this subsystem will be expanded to include channels and in-app notifications.

Additionally, it leads to awkward and repetitive API routes such as:

```
GET /messages/messages 
```

**Messaging** more clearly communicates the fact that this subsystem handles the entire messaging infrastructure and behavior, not just individual emails. It aligns with terminology commonly used in messaging systems, queues, and enterprise communications.

This name also aligns with clear and discoverable RESTful conventions. For example:

```
GET /messaging/templates/{id}
GET /messaging/mailouts/{id}
GET /messaging/deliveries
```


### Rationale for Progress instead of Record

The Progress API subsystem manages everything within the Records UI toolkit. The name Progress better reflects its purpose in tracking learner and user advancement.

**Records** is too generic and very ambiguous. Its meaning (especially without the additional context of subcomponents within Records) is unclear, especially when viewed alongside similarly vague terms like report, entry, or status. It is very easily misinterpreted as:

* A generic record of any type (e.g., user record, database row)
* A low-level data entity or audit log
* The record keyword in C#

This ambiguity limits its discoverability and does not adequately express the subsystem's role in managing:

* User advancement through courses, activities, and tasks
* Achievements, completion states, and grades
* Time-based metrics and progression analytics

**Progress** much more accurately conveys the intent of the subsystem:

* Managing learning progress over time
* Aligning with learning, workflow, and competency subsystems
* Providing insights for dashboards, reports, and tracking

This name also improves alignment with domain-specific language in education, training, and performance systems, and enhances the overall semantic clarity of the system's API surface. For example:

```
GET /progress/achievements
GET /progress/credentials
GET /progress/gradebooks
GET /progress/logbooks
```


### Rationale for Workflow Forms instead of Surveys

The Workflow API subsystem manages everything within the Surveys UI toolkit. Forms are housed within Workflow rather than in a standalone subsystem because surveys are fundamentally a type of structured data collection that supports broader workflow processes.

**Surveys** as a standalone subsystem is too narrow and disconnected. It is easily misinterpreted to mean:

* Traditional questionnaires only
* Static data collection forms
* Standalone surveys disconnected from system workflows

Additionally, it leads to awkward and repetitive API routes like:

```
GET /surveys/surveys
```

**Workflow Forms** better reflects the intent and operational context of this functionality:

* Forms are triggers, inputs, or checkpoints within larger workflow processes
* Survey responses feed into approvals, notifications, and downstream actions
* Data collection is rarely an end in itself; it initiates or advances work

Placing forms within Workflow also avoids entity name collisions across subsystems (e.g., `evaluation.Form` vs `survey.Form`) and reinforces the principle that each aggregate name should be unique across the platform.

This structure aligns with clear and discoverable RESTful conventions. For example:

```
GET /workflow/forms/{id}
GET /workflow/forms/{id}/submissions
GET /workflow/submissions
```


### Rationale for Workspace instead of Site

The Workspace API subsystem manages everything within the Sites UI toolkit. The name Workspace better reflects its functional role and to improves clarity within a system that is oriented to training, education, and human resources.

**Site** is too generic and misleading; it is easily misinterpreted as:

* A physical location for a classroom training session or exam event
* A public-facing website or marketing CMS
* A static collection of content pages
* A system for managing SEO, theming, or publishing

This causes confusion given the subsystem's actual responsibilities, which include:

* Defining navigational structures and feature launchpads
* Assembling application screens and layouts
* Providing role-specific and tenant-specific entry points into other subsystems

**Workspace** more accurately conveys the purpose of the subsystem:

* Orchestrating the runtime environment in which users interact with the system
* Managing how features are grouped, displayed, and accessed across roles and organizations
* Representing the contextual UI structure of the platform as a whole

This name aligns with enterprise software conventions and supports future extensibility for multi-tenant, multi-role, and modular user interface contexts. Also, it provides clear and meaningful API endpoints. For example:

```
GET /workspace/navigation
GET /workspace/sites
GET /workspace/portals
GET /workspace/pages
GET /workspace/pages/{id}/launch-cards
```