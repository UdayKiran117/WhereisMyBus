# API v1 documentation

Swagger is generated from the implementation in Development at `/swagger`. Public API transport models have camelCase JSON property names. REST versioning uses the `/api/v1` prefix. Authentication and tracking routes from the master prompt are not exposed yet.

| Method | Path | Current response |
|---|---|---|
| GET | /api/v1/app-info | App metadata and driverTrackingEnabled false |
| GET | /api/v1/buses?limit=50 | Active bus list, max 100 |
| GET | /api/v1/buses/{id} | Bus or 404 |
| GET | /api/v1/buses/search?registrationNumber=TS09AB1234 | Exact normalized match list |
| GET | /api/v1/buses/by-route?fromId=1&toId=3 | Directional route match list |
| GET | /api/v1/locations/search?query=Hyder | Up to 50 matching locations |
| GET | /api/v1/routes | Up to 100 active routes |
| GET | /api/v1/routes/{id} | Route or 404 |
| GET | /health/live | Process liveness independent of MySQL |
| GET | /health/ready | MySQL connectivity readiness |

Bus response example:

```json
[{"id":1,"registrationNumber":"TS09AB1234","operatorName":"Pilot Transport","displayName":"Hyderabad to Warangal","trackingStatus":"Unavailable"}]
```

An unknown registration returns HTTP 200 with an empty list. Invalid input returns 400 problem details. Database failure returns 503. Unexpected failures return 500 with a generic message and trace ID. Rate limit is 120 catalog requests per minute per remote IP; excess requests return 429.

Behind a reverse proxy, configure trusted forwarded headers before rate limiting to distinguish client IP addresses safely. No forwarded-header trust is enabled by default.

## Contract evolution

Additive optional fields remain in v1. Breaking changes require a new version. Independently update mobile models from generated OpenAPI and run compatibility tests. This package includes an exported v1 OpenAPI document when generated during verification.
