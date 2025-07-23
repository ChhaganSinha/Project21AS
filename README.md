# Project21AS

This repository contains the Project21AS application.

## Fingerprint Sensor Selection

The server reads the `FingerprintSensor` value from `appsettings.json`. Set it to
`"FM220U"` to enable the FM220U-LI middleware; otherwise the default MFS100
integration is used. The client loads the appropriate JavaScript driver at runtime
via `/api/config/fingerprint-sensor`.

