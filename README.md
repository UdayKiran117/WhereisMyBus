# Where Is My Bus

Milestone 1 source delivery for the supplied master development prompt.

**Implemented:** separate server and Android client solutions, clean architecture, a complete initial database schema, public catalog APIs, passenger number/route search, MAUI shell and theme, three-language search labels, dependency injection, automated foundation tests, Docker configuration and setup instructions.

**Verified:** server solution build; 29 server unit tests, 7 HTTP integration tests with mocked repositories, 3 architecture tests, and 11 mobile core tests.

**Blocked:** Android build and emulator verification. The workspace has no Android SDK or complete JDK. Automated dependency installation failed certificate validation while fetching the SDK manifest. Docker and MySQL are also unavailable here, so database initialization and real database integration are unverified.

**Not yet implemented:** driver authentication, administration writes, secure token lifecycle, GPS collection, foreground service, tracking ingestion, SignalR, Google Maps, ETA, offline GPS queue, favorites, voice features, Android UI automation, production signing and release deployment.

The source prompt requires the current milestone to compile and pass its applicable tests before proceeding. Milestone 1 remains awaiting Android and database verification. This package is a foundation delivery, not a production release.

## Open in Visual Studio

1. Extract this archive to a short path, for example `D:\Projects\WhereIsMyBus`.
2. Install a Visual Studio version supporting .NET 10 with the .NET MAUI Android workload, Android SDK and JDK recommended by the workload.
3. Open `Server/WhereIsMyBus.Server.sln` for the API.
4. Open `Client/WhereIsMyBus.Client.sln` separately for Android.
5. Follow `Server/README.md` and `Client/README.md`.

## First working scenario

Start the API and MySQL using the server's development Compose configuration. Launch the Android emulator. Search for `TS09AB1234`, or choose Hyderabad as From and Warangal as To. Seeded buses should appear with **Tracking unavailable**. There is no seeded live location.

## Contents

- `Server/`: independently buildable API solution, tests, SQL and Docker files.
- `Client/`: independently maintained Android solution and platform-neutral tests.
- `Documentation/`: architecture, API contract, setup, test cases and implementation status.
- `tools/verify_structure.py`: static packaging and project-reference checks.

API transport models are separately owned on each side. The server is not referenced by the client. Use the OpenAPI contract as the integration boundary.
