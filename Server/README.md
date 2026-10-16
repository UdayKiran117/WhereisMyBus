# Server setup

## Prerequisites

.NET 10 SDK; Docker Desktop with Linux containers, or MySQL 8.4. The SDK pin permits stable .NET 10 feature-band updates. No Android workload is needed for this solution.

## Development using Docker

From this `Server` folder in PowerShell:

```powershell
Copy-Item .env.example .env
notepad .env
# Replace both password placeholders with different random values.
docker compose up --build -d
docker compose ps
docker compose logs api
```

Open `http://localhost:5080/swagger`. Check `http://localhost:5080/health/ready` and search `http://localhost:5080/api/v1/buses/search?registrationNumber=TS09AB1234`.

MySQL uses host port **3307** and container port **3306**. The API uses `mysql:3306` inside Compose. Change `MYSQL_HOST_PORT` if 3307 is also occupied. API and database ports bind only to the development computer.

The SQL scripts run only on a new database volume. Do not delete an existing volume to apply a schema update; use a reviewed migration and backup. `900_RollbackSchema.sql` is destructive and is excluded from automatic startup.

## Run API from Visual Studio or CLI

Run only MySQL in Docker:

```powershell
docker compose up -d mysql
Copy-Item src/WhereIsMyBus.API/appsettings.Local.example.json src/WhereIsMyBus.API/appsettings.Local.json
notepad src/WhereIsMyBus.API/appsettings.Local.json
# Set the same MYSQL_PASSWORD chosen in .env.
dotnet restore WhereIsMyBus.Server.sln
dotnet build WhereIsMyBus.Server.sln -c Release
dotnet run --project src/WhereIsMyBus.API --launch-profile http
```

Alternatively set `Database__ConnectionString` as an environment variable. Do not commit your local configuration. Environment values take precedence over local JSON. Production database connections should require verified TLS.

If using an existing MySQL instance, apply scripts `001`, `002`, `003`, `004` in order on a fresh database using a migration account. Create a separate runtime account with SELECT permission for this read-only milestone. Compose's `busapp` account is for development.

## Tests

```powershell
dotnet test tests/WhereIsMyBus.UnitTests
dotnet test tests/WhereIsMyBus.IntegrationTests
dotnet test tests/WhereIsMyBus.ArchitectureTests
dotnet test tests/WhereIsMyBus.UnitTests --collect:"XPlat Code Coverage"
```

HTTP integration tests use a mocked catalog repository and the real MVC pipeline. They do not prove real database operation. SQL initialization must be checked separately using MySQL.

On restricted build agents, use `-m:1 -p:UseSharedCompilation=false` to avoid MSBuild named-pipe workers.

## Database smoke verification

After startup, execute queries in `database/005_VerificationQueries.sql` manually through your MySQL client. Check that there are 15 tables, 2 buses, 3 locations and one route. Both Hyderabad-to-Warangal and Hyderabad-to-Bhongir should return 2 buses; reverse directions should return none.

## Linux deployment

This Compose file is development configuration, with Swagger enabled and loopback HTTP. It is not a production deployment file. Production deployment requires a trusted HTTPS reverse proxy, database TLS, restricted database credentials, backups, monitoring, reviewed secrets, and the authentication/tracking milestones. See `Documentation/docs/10_Deployment_Guide.md`.
