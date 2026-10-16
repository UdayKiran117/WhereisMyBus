# Automated test cases

50 foundation tests passed. See the execution report for suite totals and exact limitations. The tables below preserve all requested scenarios; they are a roadmap, not a claim that all scenarios are implemented.

## Requested backend cases

| Test ID | Test Scenario | Expected Result |
|---|---|---|
| API-001 | Search using valid bus registration number | Matching bus returned |
| API-002 | Search using unknown registration number | Not Found or empty result |
| API-003 | Search using empty registration number | Validation error |
| API-004 | Search using From and To locations | Matching routes returned |
| API-005 | Search with identical From and To | Validation error |
| API-006 | Driver logs in with valid credentials | Authentication succeeds |
| API-007 | Driver logs in with invalid credentials | Authentication rejected |
| API-008 | Driver attempts to track unassigned bus | Forbidden |
| API-009 | Driver starts valid tracking session | Session created |
| API-010 | Driver starts duplicate active session | Duplicate prevented |
| API-011 | Valid GPS coordinates submitted | Location accepted |
| API-012 | Invalid latitude or longitude submitted | Validation error |
| API-013 | GPS update with expired session | Update rejected |
| API-014 | Valid location received | Latest location updated |
| API-015 | Location saved successfully | Authorized SignalR event published |
| API-016 | Driver stops tracking | Session closed |
| API-017 | Tracking unavailable | Correct status returned |
| API-018 | Database operation fails | Controlled error returned |
| API-019 | JWT token expires | Unauthorized |
| API-020 | Refresh token reused after rotation | Token rejected |
| API-021 | Concurrent tracking start requests | Only one session succeeds |
| API-022 | Duplicate location update received | Idempotency maintained |
| API-023 | Older GPS location arrives after newer one | Latest location not overwritten |
| API-024 | Unauthorized tracking subscription | Access denied according to policy |
| API-025 | Database transaction fails | Changes rolled back |

## Requested mobile cases

| Test ID | Test Scenario | Expected Result |
|---|---|---|
| MOB-001 | Passenger opens application | Home ViewModel initializes |
| MOB-002 | Passenger searches valid bus number | Search API invoked |
| MOB-003 | Passenger submits empty search | Validation message displayed |
| MOB-004 | Passenger searches by route | Correct parameters sent |
| MOB-005 | Search returns no buses | Empty state displayed |
| MOB-006 | Passenger selects a bus | Tracking screen opens |
| MOB-007 | Location update event received | Map state updates |
| MOB-008 | SignalR disconnects | Reconnection state displayed |
| MOB-009 | Driver logs in successfully | Driver dashboard opens |
| MOB-010 | Driver starts tracking | Tracking service invoked |
| MOB-011 | Location permission denied | Tracking blocked with explanation |
| MOB-012 | Driver stops tracking | Tracking service stopped |
| MOB-013 | Network connection fails | Offline state displayed |
| MOB-014 | Language changes to Telugu | Localized resources displayed |
| MOB-015 | Driver selects unassigned bus | Selection rejected |
| MOB-016 | GPS update fails | Retry mechanism invoked |
| MOB-017 | Driver logs out | Tokens cleared securely |
| MOB-018 | Stale bus location received | Delayed status displayed |
| MOB-019 | App resumes after interruption | Valid session state restored |
| MOB-020 | Driver presses Start twice | Duplicate tracking start prevented |

## Current scenario coverage

Backend API-001 through API-005 are covered at the service layer; HTTP tests cover number matching, unknown numbers, empty input and identical route validation. Route ordering SQL still needs a real MySQL test. API-017 is represented by the foundation's always-unavailable response, not a real tracking-status service. Other backend scenarios require later features and have no pass result yet.

Mobile MOB-002, MOB-003, MOB-004, MOB-005, MOB-013, MOB-014 and timeout/duplicate search handling are covered in mobile-core tests. MOB-014 covers selected translated search labels, not every screen. Driver, GPS, map and navigation scenarios are pending.

Additional foundation tests verify normalized input, cancellation forwarding, list bounds, loading cleanup, location selector loading, repeated search prevention and dependency boundaries. No network or Android hardware is used in unit tests.
