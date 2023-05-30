# Introduction
This document aims to provide you with an understanding of what an API gateway is, introduce you to Ocelot, and guide you through the configuration of routes and other advanced features.

# What is an API Gateway?
An API gateway is a service that acts as an intermediary between clients and the microservices within your application. It provides a single entry point for all incoming requests and handles various cross-cutting concerns such as routing, load balancing, authentication, authorization, caching, and monitoring.

The API gateway simplifies the client's interaction with the system by abstracting the complexity of the underlying microservices architecture. It allows you to enforce consistent policies and security measures across multiple services while providing a unified API surface for clients.

# Ocelot: An Overview
Ocelot is an open-source .NET API gateway that is built on top of the ASP.NET Core framework. It provides a simple and flexible way to configure routing and other features typically required in an API gateway.

Key Features of Ocelot:

* **Routing**: Ocelot allows you to define various routing strategies, such as HTTP verb-based routing and path-based routing, to direct incoming requests to the appropriate microservices.
* **Load Balancing**: Ocelot supports different load balancing algorithms to distribute the traffic across multiple instances of a microservice, enhancing scalability and fault tolerance.
* **Authentication and Authorization**: Ocelot enables you to enforce authentication and authorization policies for incoming requests, providing a secure gateway for your microservices.
* **Rate Limiting**: With Ocelot, you can configure rate limiting rules to control the number of requests a client can make within a given time window.
* **Caching**: Ocelot allows you to cache responses from your microservices, reducing latency and improving overall performance.
* **Monitoring and Logging**: Ocelot integrates with various logging and monitoring solutions, making it easier to track and analyze the gateway's behavior.

# Configuration
Ocelot's configuration is typically stored in JSON files:
* `Ocelot/ocelot.global.json`: Contains global configuration
* `Ocelot/ocelot.company.api.json`: Contains routes for the `company-api`
* `Ocelot/ocelot.project.api.json`: Contains routes for the `project-api`

For each new service we can create another `Ocelot/ocelot.<servicename>.json` file

ocelot.global.json:
```json
{
  "GlobalConfiguration": {
    "BaseUrl": "https://localhost:7068",
    "ServiceDiscoveryProvider": {
      "Host": "localhost",
      "Port": 8500,
      "Token": "8bd101ce-8200-f02a-1f6c-73609cf88277",
      "Type": "Consul"
    }
  }
}
```

ocelot.company.api.json (extract):
```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/v1/Company",
      "DownstreamScheme": "https",
      "UpstreamPathTemplate": "/api/v1/Company",
      "UpstreamHttpMethod": [
        "Get"
      ],
      "ServiceName": "company-api",
      "LoadBalancerOptions": {
        "Type": "LeastConnection"
      },
      "DangerousAcceptAnyServerCertificateValidator": true,
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": [
          "read:company"
        ]
      }
    }
  ]
}
```

Explanation for above config:

* **Routes**: This is an array that contains one or more route definitions.
* **DownstreamPathTemplate**: Specifies the downstream path template, which is the path used to forward requests to the downstream service(s). In this example, requests with the path /api/v1/Company will be forwarded.
* **DownstreamScheme**: Specifies the scheme (HTTP or HTTPS) to be used when forwarding requests to the downstream service(s). In this example, HTTPS is used.
* **UpstreamPathTemplate**: Specifies the upstream path template, which is the path used by clients when making requests to the API gateway. In this example, requests with the path /api/v1/Company will be received by the API gateway.
* **UpstreamHttpMethod**: Specifies the HTTP method(s) that the API gateway should expect for the incoming requests. In this example, only GET requests are expected.
* **ServiceName**: Specifies the name of the downstream service. This can be any meaningful name you assign to identify the service.
* **LoadBalancerOptions**: Specifies the load balancing options for the downstream service. In this example, the load balancing algorithm used is LeastConnection, which distributes requests evenly based on the number of active connections.
* **DangerousAcceptAnyServerCertificateValidator**: A flag indicating whether the API gateway should accept any SSL/TLS certificate from the downstream service(s) without validation. Setting this to true can be risky as it bypasses certificate validation.
* **AuthenticationOptions**: Specifies the authentication options for the route. In this example, the authentication provider is set to Bearer, indicating that the API gateway expects a bearer token for authentication. The AllowedScopes property defines the scopes required for accessing this route. In this case, the user must have the read:company scope to access the /api/v1/Company endpoint.

