# Functional requirements

## Current passenger behavior

A passenger can enter a registration number, search, see returned buses, load route locations, and search between two different locations. Number searches are exact after removing spaces/hyphens and converting to uppercase. Route searches respect ordered stops, including intermediate stops. Results always show tracking unavailable in the foundation.

Validation prevents empty/malformed registration numbers, invalid IDs, identical route endpoints, and unbounded list limits. Offline, timeout and empty-result states have distinct messages. Passenger APIs are public and rate limited.

## Planned driver behavior

A driver signs in, chooses an assigned bus/route, confirms location sharing, grants permission and starts an authorized tracking session. Stop ends both the server session and the Android collection service. One active bus session and one active driver session are enforced by the database in the planned write path.

## Planned tracking behavior

Each GPS update carries an update ID, session ID, capture timestamp, coordinates, speed, heading and accuracy. The server verifies ownership and assignment, stores history, advances latest location only for newer samples, and broadcasts committed current data. An old queued location must never be marked live. This write path and UI are not yet implemented.

## Optional enhancements

Voice search, spoken guidance, ETA, favorites, advanced offline search and an administrative web dashboard follow the core milestones. This package makes no claim that these features exist.
