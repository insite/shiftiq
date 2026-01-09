# Shift API

Here is the list of components in the Shift API:

* **Application Feature Components**
  * Assessment
  * Billing (Sales)
  * Booking (Calendar/Event/Schedule)
  * Competency (Standard)
  * Content
  * Directory (Contact)
  * Feedback (Survey)
  * Job
  * Learning (Course)
  * Messaging (Message)
  * Progress (Record)
  * Reporting
  * Workflow
  * Workspace (Site)
  
* **Plugin Components**
  * Integration
  * Variant

* **Utility Components**
  * Metadata
  * Platform
  * Security
  * Timeline

* **Special-Purpose Components**
  * Internal
  * Orchestration


## Special-Purpose Components

### Internal

**Internal** is a special-purpose subsystem that encapsulates non-public, platform-facing functionality used exclusively for system maintenance, diagnostics, testing, and administrative operations. It contains tools, endpoints, and workflows that support internal development, staging, and operational activities, but are not intended for tenant access or external API consumers. The Internal component provides isolated access to infrastructure-level behaviors without exposing them in the public API surface, and should be treated as privileged and access-restricted in all environments.

### Orchestration

**Orchestration** is a special-purpose subsystem responsible for aggregating and composing data from multiple components to deliver unified, session-aware views for client applications. It acts as a bridge between feature-specific subsystems and the user interface, assembling contextual models such as user session state, dashboard metadata, or cross-component summaries. Orchestration is not responsible for business logic or data ownership, but instead coordinates and shapes data from underlying services to support runtime experience delivery, particularly for dynamic front-end rendering and role-based navigation.


## API Component Aliases for UI Toolkits


### Rationale for Billing instead of Sales

The Billing API component manages everything within the Sales UI toolkit. The name Billing better reflects its functional purpose, which improves clarity for developers and product stakeholders.

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

The Booking API component manages everything within the Events UI toolkit. The name Booking better reflects its true functional purpose and improves clarity for developers and product stakeholders.

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

The Competency API component manages everything within the Standards UI toolkit. The name Competency better represents its functional purpose and enhance clarity for developers, domain experts, and product stakeholders.

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

The Directory API component manages everything within the Contacts UI toolkit. The name Directory more accurately reflects its scope and improves semantic clarity for developers and product stakeholders.

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


### Rationale for Feedback instead of Surveys

The Feedback API component manages everything within the Surveys UI toolkit. The name Feedback better captures its purpose and improves clarity for developers and product stakeholders.

**Surveys** is too specific and limited in scope. It is easily misinterpreted to mean:

* Traditional questionnaires only
* Static data collection forms
* Standalone surveys disconnected from system workflows

Additionally, it leads to awkward and repetitive API routes like:

```
GET /surveys/surveys
```

> Technical Note: API routes like surveys/forms are intentionally avoided because they introduce the potential for duplication of entity names. Although C# namespaces and SQL Server schemas allow `assessment.Form` and `survey.Form` to co-exist (for example), this is discouraged to avoid overloading the names of aggregates, commands, and changes.

**Feedback** better reflects the intent and evolving capabilities of this subsystem:

* Collecting structured and unstructured responses from users
* Supporting feedback workflows tied to assessments, courses, and other content
* Enabling analytics and improvement loops based on user input

This name aligns with clear and easily readable RESTful conventions and improves API discoverability. For example:

```
GET /feedback/surveys/{id}
GET /feedback/surveys/{id}/responses
GET /feedback/responses
```


### Rationale for Learning instead of Courses

The Learning API component manages everything within the Courses UI toolkit. The name Learning better reflects the broader scope of functionality and improves clarity and extensibility for developers and product stakeholders.

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

The Messaging API component manages everything within the Messages UI toolkit. The name Messaging more accurately represents the subsystem's purpose and improves clarity for developers and administrators.

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

The Progress API component manages everything within the Records UI toolkit. The name Progress better reflects its purpose in tracking learner and user advancement.

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


### Rationale for Workspace instead of Site

The Workspace API component manages everything within the Sites UI toolkit. The name Workspace better reflects its functional role and to improves clarity within a system that is oriented to training, education, and human resources.

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