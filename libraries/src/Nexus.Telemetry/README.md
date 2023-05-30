# How to Use

# Overview
![Overview](nexus-tracing.png)

## Configuration

Ensure the following settings exist in appsettings.json / Environment variables / Consul KV:

```json
{
  "TelemetrySettings": {
    "Endpoint": "http://jaeger:4317",
    "ServiceName": "<service-name>",
    "ServiceVersion": "<service-version>",
    "EnableAlwaysOnSampler": false,
    "SampleProbability": 0.2,
    "EnableConsoleExporter": false
  }
}
```

Notes:
* Telemetry works on a probability basis. Each call has a `SampleProbability` probability of being traced
* Tracing has a performance cost so it is not a good idea to enable `AlwaysOnSampler` an environment other than local development
* `EnableConsoleExporter` exports the same traces to the local console in addition to jaeger

## Usage

While registering services, call:

```
services.AddCoreTelemetry(configuration);
```
