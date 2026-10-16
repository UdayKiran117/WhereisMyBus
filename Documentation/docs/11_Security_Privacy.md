# Security and privacy

This foundation exposes only a public bus/location catalog. No authentication endpoint, administrative write or GPS ingestion endpoint exists. A driver page does not grant access to a protected service. No GPS permissions are requested and no location data is collected.

Dapper parameters prevent request values from becoming SQL text. Constant column fragments are the only interpolated SQL. Input lengths and result counts are bounded. Rate limiting is enabled. Unexpected responses do not disclose SQL or credentials.

Development configuration permits local HTTP to the emulator host only. Release networking requires HTTPS and uses an invalid host until configured. Certificate validation is never bypassed. Local secrets files, environment files and signing keys are ignored by source control and excluded from the source archive.

Authentication, secure device token storage, refresh rotation/reuse detection, administrator authorization, consent records, session ownership, GPS anti-spoofing controls, history retention and audited writes remain later work. The schema provides columns and constraints for them, but schema presence is not an implemented security feature.

Before production, review dependency advisories, public tracking visibility, operator permissions, driver disclosure, retention duration, account recovery and Google Play requirements. No production privacy policy or release approval is claimed by this foundation.
