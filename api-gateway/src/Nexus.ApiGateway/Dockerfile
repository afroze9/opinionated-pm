FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["libraries/src/Nexus.Auth/Nexus.Auth.csproj", "libraries/src/Nexus.Auth/"]
COPY ["libraries/src/Nexus.Configuration/Nexus.Configuration.csproj", "libraries/src/Nexus.Configuration/"]
COPY ["libraries/src/Nexus.Core/Nexus.Core.csproj", "libraries/src/Nexus.Core/"]
COPY ["libraries/src/Nexus.Logging/Nexus.Logging.csproj", "libraries/src/Nexus.Logging/"]
COPY ["libraries/src/Nexus.Persistence/Nexus.Persistence.csproj", "libraries/src/Nexus.Persistence/"]
COPY ["libraries/src/Nexus.Framework.Web/Nexus.Framework.Web.csproj", "libraries/src/Nexus.Framework.Web/"]
COPY ["libraries/src/Nexus.Management/Nexus.Management.csproj", "libraries/src/Nexus.Management/"]
COPY ["libraries/src/Nexus.Telemetry/Nexus.Telemetry.csproj", "libraries/src/Nexus.Telemetry/"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]

COPY ["api-gateway/src/Nexus.ApiGateway/nuget.config", "api-gateway/src/Nexus.ApiGateway/"]
COPY ["api-gateway/src/Nexus.ApiGateway/Nexus.ApiGateway.csproj", "api-gateway/src/Nexus.ApiGateway/"]

RUN dotnet restore "api-gateway/src/Nexus.ApiGateway/Nexus.ApiGateway.csproj"

COPY . .

WORKDIR "/src/api-gateway/src/Nexus.ApiGateway"
RUN dotnet build "Nexus.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexus.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.ApiGateway.dll"]
