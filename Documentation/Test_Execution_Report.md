# Foundation test execution report

Executed in the supplied Linux workspace on 13 October 2026 using .NET SDK 10.0.401 and runtime 10.0.12.

| Suite | Passed | Failed | Skipped |
|---|---:|---:|---:|
| Server unit | 29 | 0 | 0 |
| Server HTTP integration with mocked repository | 7 | 0 | 0 |
| Server architecture | 3 | 0 | 0 |
| Mobile core unit | 11 | 0 | 0 |
| Total | 50 | 0 | 0 |

The server solution compiled successfully. Mobile.Core and its unit-test project compiled successfully. Package restores succeeded for the server, mobile core, and MAUI project.

The full Android application build failed XA5300 because the Android SDK was absent and the available Java installation lacked required JDK tools. An InstallAndroidDependencies attempt failed certificate validation when fetching the SDK manifest. No APK, emulator run, Android UI automation or XAML runtime verification was completed.

No Docker or MySQL executable is available in this workspace. SQL schema initialization and real MySQL integration remain unverified. HTTP tests use a mocked repository and do not prove Dapper/MySQL behavior.

Raw TRX results and Cobertura coverage XML files are in `TestEvidence/`. Coverage describes the foundation only; it does not measure unimplemented milestones or prove correctness. See `Coverage_Summary.md` for assembly-specific figures.

## Process smoke test

The API started under Kestrel. Development Swagger generated the archived v1 OpenAPI document with 8 public catalog/app-info paths. Liveness returned HTTP 200. Readiness returned HTTP 503 against an intentionally unavailable database, confirming the unhealthy dependency state. No successful database query is claimed.

## Static Android item verification

MSBuild item evaluation found exactly one Android network-policy resource after removing a duplicate explicit inclusion. This verifies resource inclusion, not APK compilation.
