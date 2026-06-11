# Shift.Hub

Shift.Hub is the consolidated API that replaces four historically separate deployments:

| Legacy project      | Legacy URL                          | Shift.Hub route prefix |
|---------------------|-------------------------------------|------------------------|
| Engine.Api          | api.shiftiq.com/google              | /google                |
| Engine.CssToHtml    | api.shiftiq.com/premailer           | /premailer             |
| Engine.ImageMagick  | api.shiftiq.com/imagemagick         | /imagemagick           |
| Engine.Scorm        | api.shiftiq.com/scorm               | /scorm                 |

All endpoints, request bodies, and response shapes are preserved. Only the base URL used by clients changes.

## Structure

- `Common/`    shared application setup, health/version base controllers, Swagger and Sentry wiring (was Engine.Common).
- `Google/`    Google Translate, location lookup, translation cache, EF DbContext, rate limiter (was Engine.Api).
- `Premailer/` PreMailer.Net CSS inlining (was Engine.CssToHtml).
- `ImageMagick/` NetVips-backed image info/adjust endpoints (was Engine.ImageMagick).
- `Scorm/`    Rustici SCORM Cloud wrapper; authenticator runs only for `/scorm` paths (was Engine.Scorm).

## Parallel deployment

Shift.Hub is designed to run side-by-side with the four legacy deployments for one release cycle. Client projects switch over by updating a single base-URL configuration value.
