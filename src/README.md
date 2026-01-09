# src: Application source code

Source code files for the project.

## What should go in src?

- Application code (frontend/backend/modules)
- Libraries and reusable components
- Entry points (e.g., `main.ts`, `index.js`)

## What should NOT go in src?

- Build artifacts
- Configuration settings
- Static assets or public resources

## Subfolders

### api

Backend services and applications that expose data and functionality via HTTP APIs. These projects handle server-side logic, routing, and integrations.

### cli

Command-line interfaces that provide tooling and automation for developers and operators. These apps interact with other services, scripts, and local resources.

### lib (or library)

Libraries that encapsulate reusable logic, types, and domain models — intended for use in other projects rather than being used directly.

### test

Libraries and applications specifically designed for testing, validation, and benchmarking to ensure code correctness, performance, and integration coverage.

### web (or ui)

Frontend user-facing applications and sites rendered in a browser.