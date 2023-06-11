# Nexus Template

## Getting Started
### Prerequisites
* Auth0
* Docker
* DotNET 7 SDK

### Setting up Auth0

#### Create Backend Application

* Under Applications > Applications, create a new application with the following settings:
    * Name: `Project Management Backend`
    * Application Type: Regular Web Application
    * Allowed Callback URLs: `http://localhost:8012/login/oauth2/code/auth0`
    * Grant Types: Authorization Code, Refresh Token, Implicit, Client Credentials

#### Create a Frontend Application

* Under Applications > Applications, create a new application with the following settings:
    * Name: `Project Management Frontend`
    * Application Type: Single Page Application
    * Allowed Callback URLs: `http://localhost:3000`
    * Allowed Logout URLs: `http://localhost:3000`
    * Grant Types: Authorization Code, Refresh Token, Implicit

#### Create an API

* Under Applications > APIs, create a new API with the following settings:
    * Name: `Project Management`
    * Identifier: `projectmanagement`
    * Signing Algorithm: `RS256`
* Under Permissions tab of the API, create the following permissions:
    * `read:company`
    * `write:company`
    * `read:project`
    * `write:project`
    * `read:project`
    * `write:project`
    * `update:project`
    * `delete:project`
* Under the Machine to Machine Applications tab, Authorize the Backend Application created above to access the API and
  assign the permissions created above.

### Setting up the Solution

1. Install the nexus-tool:
```powershell
dotnet tool install --global nexus-tool
```

2. From an empty directory run:
```powershell
nexus init "HelloWorld"
```

3. Add your own service (optional):
```powershell
nexus add service "people"
```
A detailed guide on how to add new services can be found [here](docs/add-service.md)

4. Run the supporting services:
```powershell
nexus run local
```

5. Develop/Run services from the IDE as necessary


## Links
* [Architecture Overview](docs/architecture-overview.md)
* [Project Scope](docs/scope.md)
* [Known Issues](docs/known-issues.md)
* [Adding new Services](docs/add-service.md)