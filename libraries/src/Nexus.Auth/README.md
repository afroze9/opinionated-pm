# How to Use

# Overview
![Overview](nexus-auth.png)

## Configuration

Ensure the following settings exist in appsettings.json / Environment variables / Consul KV:

```json
{
  "Auth0Settings": {
    "Authority": "https://teamly.us.auth0.com",
    "Audience": "projectmanagement"
  }
}
```

## Usage

While registering services, call:

```
services.AddCoreAuth(configuration, RESOURCE_NAME);
```

Here `RESOURCE_NAME` is whatever API Resource you need to add basic CRUD policies to.
Below will add `read:company`, `write:company`, `update:company`, and `delete:company` policies for `company` resouce.

```
services.AddCoreAuth(configuration, "company");
```

Additionally, you can use a custom list of actions.
Below will add `search:company` and `print:company`policies for `company` resouce.

```
List<string> actions = new List<string>() { "search", "print" };
services.AddCoreAuth(configuration, "company", actions);
```

