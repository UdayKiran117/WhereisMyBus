# Integration and end to end verification

Seven HTTP tests exercise the real ASP.NET pipeline through WebApplicationFactory with a mocked catalog repository: liveness, valid public search, empty search problem details, identical route endpoints, unknown registration, development Swagger and app-info feature flags.

## Required database verification

Run MySQL 8.4 and initialize scripts 001 through 004. Verify seed counts and directional searches using `005_VerificationQueries.sql`. Test actual Dapper record mapping and indexes using the live public APIs. Database integration is unverified here because neither MySQL nor Docker is available.

## Required Android foundation verification

Build the client using a supported Android toolchain. On an emulator, verify launch, English/Telugu/Hindi search labels, registration normalization, route picker loading, empty search, invalid input, offline message, dark theme and large fonts. API connection defaults to 10.0.2.2:5080.

Android UI automation is not implemented yet; no empty UI-test project or fake passing tests are included. Choose a suitable emulator automation harness in Milestone 7 and provide independent UI-test commands then.

## Planned integration scenarios

Authentication/token rotation, assignment validation, concurrent tracking starts, transactional GPS writes, idempotent retries, delayed samples, SignalR delivery, and driver service interruption recovery become mandatory as those features are implemented. Tests must use disposable database fixtures, avoiding production data.
