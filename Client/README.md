# Android client setup

## Prerequisites

.NET 10 SDK, .NET MAUI Android workload, Android SDK API 36 and its build tools, and a complete JDK compatible with the installed .NET Android workload. Use Visual Studio's supported Android toolchain setup on Windows.

```powershell
dotnet workload install maui-android
dotnet workload list
```

The client targets `net10.0-android`, with Android API 24 as the minimum supported device version. Actual support and toolchain requirements are documented by Microsoft:

- https://learn.microsoft.com/dotnet/maui/supported-platforms?view=net-maui-10.0
- https://learn.microsoft.com/dotnet/maui/get-started/installation?view=net-maui-10.0

## Build and run

1. Start the server using `Server/README.md`.
2. Open `WhereIsMyBus.Client.sln` separately in Visual Studio.
3. Set `WhereIsMyBus.Mobile` as the startup project.
4. Choose an Android emulator and use Debug.
5. Build and run.

```powershell
dotnet restore WhereIsMyBus.Client.sln
dotnet build src/WhereIsMyBus.Mobile -f net10.0-android -c Debug
# Requires a running emulator/device:
dotnet build src/WhereIsMyBus.Mobile -f net10.0-android -c Debug -t:Run
```

`Services/EndpointSettings.cs` uses `http://10.0.2.2:5080/` in Debug. That address reaches your Windows computer from the Android emulator. It is not the correct URL for a physical phone.

For a physical phone, use a reachable HTTPS development API host and update the endpoint. The debug network policy permits HTTP only to 10.0.2.2 and localhost. Release defaults to an intentionally invalid HTTPS host until the owner supplies the real API URL. Do not bypass certificate validation.

If CLI tooling cannot discover your SDK/JDK, pass the installed directories as `-p:AndroidSdkDirectory="..." -p:JavaSdkDirectory="..."`.

## Test without Android

```powershell
dotnet test tests/WhereIsMyBus.Mobile.UnitTests
dotnet test tests/WhereIsMyBus.Mobile.UnitTests --collect:"XPlat Code Coverage"
```

These tests reference only `Mobile.Core` and require neither the Android SDK nor an emulator. Avoid restoring the entire client solution when only running these tests on Linux.

## Current behavior

Passenger search and location selectors connect to API v1. English, Telugu and Hindi search labels are available. Driver tab reports that driver features are not yet available. No location permission is requested and no GPS collection occurs.

A full Android build, XAML runtime layout, font scaling and device network access still require verification. Maps and location sharing are later milestones.

## APK and AAB

This delivery does not include an APK because the Android build could not be verified here. Once the milestone is verified locally, Debug builds produce a development APK. Release APK/AAB signing and publication are Milestone 8; do not distribute this foundation as a production app.
