# Implementation status

| Milestone | Scope | Status |
|---|---|---|
| 1 | Solutions, DI, SQL, API foundation, MAUI shell, initial tests, setup | Source implemented; server/core verified; Android and database verification blocked |
| 2 | Driver login, JWT/refresh, secure master-data writes | Not implemented |
| 3 | Full passenger search screens, result navigation | Core search slice included in M1; map navigation and complete screen flow pending |
| 4 | Foreground GPS, permissions, sessions, transactional ingestion | Not implemented |
| 5 | Google Maps and SignalR | Not implemented |
| 6 | Complete localization, accessibility, optional voice | Search labels/theme groundwork included; full pass pending |
| 7 | Full test matrix, real database/Android tests, security/load tests | Foundation tests included; full QA pending |
| 8 | Signed APK/AAB, production deployment and final documentation | Not implemented |

## Gate from the supplied prompt

“Do not move to the next milestone until the current milestone compiles and its applicable automated tests pass, or any blockers are explicitly reported.”

The current blocker is Android SDK/JDK availability. Automated Android dependency installation failed with certificate validation while downloading the SDK manifest. The full MAUI application has not compiled here; runtime/XAML behavior cannot be claimed. Database SQL and Compose execution also require MySQL/Docker verification.

## Next concrete action

Build `Client/WhereIsMyBus.Client.sln` on the Windows Android environment using `Client/README.md`, run the database smoke checks, and resolve any resulting compile/runtime findings before implementing authentication and tracking.
