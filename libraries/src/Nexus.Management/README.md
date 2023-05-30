# How to Use

## Configuration

Ensure the following settings exist in appsettings.json / Environment variables / Consul KV:

```json
{
  "management": {
    "endpoints": {
      "health": {
        "showdetails": "always"
      },
      "enabled": true,
      "actuator": {
        "exposure": {
          "include": [
            "*"
          ]
        }
      }
    }
  }
}
```

## Usage

While registering services, call:

```
services.AddCoreActuators(configuration);
```

## Functionality
The AddCoreActuators method performs the following tasks:

* **AddHealthActuator**: This method adds the health actuator endpoint to expose health-related information about your application. It uses the provided IConfiguration instance to configure the health check endpoints and their respective settings.
* **AddInfoActuator**: This method adds the info actuator endpoint to provide general information about your application, such as its version, description, and other custom details. The configuration is also provided through the IConfiguration instance.
* **AddHealthChecks**: This method registers health checks within the application. Health checks are used to periodically assess the health of various components and dependencies of your application, such as databases, messaging systems, or external services.
* **AddRefreshActuator**: This method adds the refresh actuator endpoint, which allows you to dynamically refresh the configuration settings of your application without the need for a complete restart. This can be useful when you need to update configuration values during runtime.
* **ActivateActuatorEndpoints**: This method activates and exposes all the actuator endpoints configured so far. It ensures that the actuator endpoints are accessible and can be accessed by authorized users or systems.