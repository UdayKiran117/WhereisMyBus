# Deployment guide

## Development

Use `Server/docker-compose.yml` with `.env` values supplied locally. Ports are loopback only. MySQL data persists in the named volume. Database health checks delay API startup. Swagger is available because the development Compose configuration selects Development.

## Production plan

Production delivery is Milestone 8. Before release, finish driver authentication, tracking authorization, secrets management, privacy controls and release signing. Deploy the API behind a trusted HTTPS proxy; use verified database TLS and least-privilege accounts. Keep Swagger restricted. Configure trusted forwarded headers, log retention, backup recovery and readiness checks.

The API Dockerfile publishes Release and runs as the .NET image's non-root user. The development Compose file has not been executed here because Docker is absent. It must be smoke tested before deployment.

## Monitoring

`/health/live` proves the process is alive; `/health/ready` currently checks MySQL connectivity only. Readiness does not verify the schema version. Structured JSON console logging records trace IDs and failure types without raw connection strings. Later milestones should add schema readiness and tracking-specific metrics.

## Android release plan

Replace the release API URL with the real HTTPS host, provide a legitimate Android-restricted Google Maps key after map implementation, and configure owner-managed signing credentials. Produce APK/AAB only from a verified release build. No signing key or release package is included now.
