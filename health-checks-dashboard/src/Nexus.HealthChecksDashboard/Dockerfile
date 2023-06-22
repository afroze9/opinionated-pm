FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["libraries/src/Nexus.Auth/Nexus.Auth.csproj", "libraries/src/Nexus.Auth/"]
COPY ["libraries/src/Nexus.Configuration/Nexus.Configuration.csproj", "libraries/src/Nexus.Configuration/"]
COPY ["libraries/src/Nexus.Common/Nexus.Common.csproj", "libraries/src/Nexus.Common/"]
COPY ["libraries/src/Nexus.Logs/Nexus.Logs.csproj", "libraries/src/Nexus.Logs/"]
COPY ["libraries/src/Nexus.Persistence/Nexus.Persistence.csproj", "libraries/src/Nexus.Persistence/"]
COPY ["libraries/src/Nexus.Framework.Web/Nexus.Framework.Web.csproj", "libraries/src/Nexus.Framework.Web/"]
COPY ["libraries/src/Nexus.Management/Nexus.Management.csproj", "libraries/src/Nexus.Management/"]
COPY ["libraries/src/Nexus.Telemetry/Nexus.Telemetry.csproj", "libraries/src/Nexus.Telemetry/"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]

COPY ["health-checks-dashboard/src/Nexus.HealthChecksDashboard/nuget.config", "health-checks-dashboard/src/Nexus.HealthChecksDashboard/"]
COPY ["health-checks-dashboard/src/Nexus.HealthChecksDashboard/Nexus.HealthChecksDashboard.csproj", "health-checks-dashboard/src/Nexus.HealthChecksDashboard/"]

RUN dotnet restore "health-checks-dashboard/src/Nexus.HealthChecksDashboard/Nexus.HealthChecksDashboard.csproj"

COPY . .

WORKDIR "/src/health-checks-dashboard/src/Nexus.HealthChecksDashboard"
RUN dotnet build "Nexus.HealthChecksDashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexus.HealthChecksDashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.HealthChecksDashboard.dll"]
