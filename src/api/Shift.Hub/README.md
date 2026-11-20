# Shift.Hub

Shift.Hub is the consolidated API that replaces three historically separate deployments:

| Legacy project      | Legacy URL                          | Shift.Hub route prefix |
|---------------------|-------------------------------------|------------------------|
| Engine.Api          | api.shiftiq.com/google              | /google                |
| Engine.CssToHtml    | api.shiftiq.com/premailer           | /premailer             |
| Engine.ImageMagick  | api.shiftiq.com/imagemagick         | /imagemagick           |

All endpoints, request bodies, and response shapes are preserved. Only the base URL used by clients changes.

## Structure

- `Common/`    shared application setup, health/version base controllers, Swagger and Sentry wiring (was Engine.Common).
- `Google/`    Google Translate, location lookup, translation cache, EF DbContext, rate limiter (was Engine.Api).
- `Premailer/` PreMailer.Net CSS inlining (was Engine.CssToHtml).
- `ImageMagick/` NetVips-backed image info/adjust endpoints (was Engine.ImageMagick).

## Authentication

Every contract route takes the `X-Hub-Api-Key` header: `/partitions` (and the legacy `/api/partitions`), `/google/*`, `/premailer/*` and `/imagemagick/*`. The hub compares it against its own `Engine:Api:ApiKey`. The `status`, `health` and `version` endpoints under each prefix never need it, so an uptime monitor needs no secret.

The rollout is soft, and deliberately so:

| The request carries | The hub |
|---|---|
| The right key | Serves it. |
| A wrong key | Answers 401. |
| No header | Serves it, and sends a warning to Sentry naming the path and the caller's address. |
| Anything, while the hub's own key is empty | Serves it, and sends a warning to Sentry. |

A missing header means an InSite partition has not been redeployed with `Engine:Api:ApiKey`; the Sentry warning is the reminder, and it repeats at most once per caller per hour. Once Sentry stays quiet, the header can become required. Clients send their own `Engine:Api:ApiKey`, so each partition and the hub it calls must carry the same value.

The route list and the header name live in `Shift.Common.Integration.HubApiKey`, which the clients and the hub share.

## Parallel deployment

Shift.Hub is designed to run side-by-side with the three legacy deployments for one release cycle. Client projects switch over by updating a single base-URL configuration value.
