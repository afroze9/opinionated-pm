# How to Use
This utilizes Consul KV to get application configuration.

## Configuration

Ensure the following settings exist in appsettings.json / Environment variables / Consul KV:

```json
{
  "ConsulKV": {
    "Url": "http://localhost:8500",
    "Token": "<token-for-service-name-kv>",
    "Key": "<service-name>/app-config"
  }
}
```

## Usage

While building the `WebApplication`, call:

```
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);        
builder.Configuration.AddCoreConfiguration();
```
