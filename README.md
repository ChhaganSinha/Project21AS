# Project21AS

This repository contains the Project21AS application.

## Fingerprint Sensor Selection

The server reads the `FingerprintSensor` value from `appsettings.json`. It is
currently set to `"FM220U"` so the FM220U-LI middleware is enabled by default.
Change the value to `"MFS100"` if you want to use the MFS100 integration. The
client loads the appropriate JavaScript driver at runtime via
`/api/config/fingerprint-sensor`.

