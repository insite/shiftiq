# Internal

The Internal component within this library contains contains classes that implement cross-cutting concerns within the library itself (e.g., validation helpers, logging, utility base classes, etc.).

Code in this folder is used **only** within this library, and must never be referenced by code outside this library.

> Technical Note: The name "Foundation" has been used in the past for this folder. The name "Foundation" is promblematic for several reasons: it is overly vague; it implies low-level plumbing shared across functional scopes; it conflicts with the use of this term in contexts related to foundational platform improvements and technical debt reduction.

The name "Internal" is specific, accurate, unambiguous, avoids namespace collisions, and avoids confusion with broader scope terminology.

When you add code to this folder, avoid catch-all subfolder names like "Utils". Be specific about intent, with subfolder names like Base, Configuration, Extensions, Helpers, Interceptors, Logging, Validation


## What should go in Internal?

Internal classes are supportive, non-public implementation details of the library. These classes:

* Are used by multiple components in the same library
* Implement cross-cutting concerns (e.g., logging, validation, configuration binding)
* Are not intended to be exposed as part of the library's public API
* Enable reuse within the scope of the library only

Examples:

* Base Classes = Abstract base types or template methods not meant to be reused outside
* Configuration = JSON/config binders, default config providers
* Converters = Value converters, internal object translators
* Extensions = Internal extension methods (e.g., .ToInvariantDate())
* Helpers = String/date/math helpers scoped only to the library
* Interceptors = AOP logic: logging, retry, audit hooks
* Validation = XValidator, rulesets, internal schema checks

Ideally, these classes should be marked internal (C# visibility) and should not appear in any consuming library's intellisense or public contract.


## What should NOT go in Internal?

Avoid putting anything in Internal that:

* Represents a domain model
* Is a core service exposed via public API
* Is part of the library's main public functionality
* Belongs to a clear architectural layer (like Composition, Features, or Services)

Misplaced Examples:

* AppDbContext = should go in /Composition
* Controller classes = should go in /Composition
* IUserRepository = should go in /Contracts or /Interfaces
* User (domain/entity model) = should go in /Models or /Domain
* UserService = should go in /Services or /Features


## Visibility Rule of Thumb

Classes in the Internal folder:

* Should be marked with the C# internal visibility modifier, if possible
* Should never be consumed outside the current library