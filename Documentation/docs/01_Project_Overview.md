# Project overview

Where Is My Bus is an Android application for passengers to find buses and, in later milestones, watch their current position. Drivers will explicitly share location from their phones, avoiding a dedicated tracking device.

The current delivery is Milestone 1. It includes the two solutions and a useful public catalog/search slice. See `../Implementation_Status.md` for feature status and `../Test_Execution_Report.md` for verification.

Passengers do not register to search. Driver and administrator functions are planned to require authentication. ETA is not yet calculated. No current GPS position is fabricated.

## Technology baseline

| Component | Selected version |
|---|---|
| Server and mobile core | .NET 10 |
| Android app | net10.0-android; device API 24 minimum |
| Microsoft.Maui.Controls | 10.0.20 |
| CommunityToolkit.Mvvm | 8.4.0 |
| MySQL | 8.4 image |
| Dapper | 2.1.66 |
| MySqlConnector | 2.4.0 |
| Swagger | Swashbuckle.AspNetCore 7.2.0 |
| Tests | xUnit 2.9.3, Moq 4.20.72, FluentAssertions 7.0.0 |
| Coverage | Coverlet collector 6.0.4 |

Versions are pinned and restored successfully for the current projects. The actual server/mobile-core test runtime was .NET SDK 10.0.401 with .NET 10.0.12. Packages are not claimed to be the latest available versions. Audit and update before deployment.
