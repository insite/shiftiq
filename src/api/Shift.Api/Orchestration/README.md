# Orchestration

The Orchestration component within this library contains high-level composition classes that depend on other parts of the library to coordinate or expose functionality. These classes act as entry points, facades, or coordinators.

In other words, classes in this folder reference other classes throughout the library to compose larger objects that provide unified functionality. The main purpose of such classes is structural: assembling, owning, or exposing parts.

> Please note this component is renamed from "Composition", which was not the clearest or most intuitive name for new collaborators. The term "Orchestration" is used frequently in domain-driven design to describe a component that coordinates multiple subsystems into cohesive runtime outputs. It clearly conveys "combining services and logic", and it is neutral enough to use in different contexts (e.g., feature, session, layout).


## What should go in Orchestration?

High-level, coordinating classes that:

* Reference or depend on multiple other components in the same library
* Assemble, orchestrate, or expose features from lower layers
* Serve as entry points or context holders

For example:

* LibraryContext = A class that exposes services, features, or data access objects
* ModuleController = Coordinates workflow across internal services or modules
* FeatureAggregator = Combines results from multiple internal operations
* AppFacade = Provides a simplified interface to internal complexity
* Bootstrapper = Initializes or wires up internal dependencies

Orchestration classes should not contain business logic, but should delegate such logic to lower-level classes.


## What should NOT go in Orchestration?

* Low-level services (XService, XValidator)
* Helper or utility classes
* Domain models or DTOs
* Isolated features


## Heuristics for inclusion in Orchestration

Ask these questions:

1. Does the class have dependencies on three or more distinct internal components?

2. Would another system or library consume this class as an entry point into the library?

3. Is this class a facade, controller, context, or initializer?

If yes, then it belongs in Orchestration.
