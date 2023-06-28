FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["health-checks-dashboard/src/Nexus.HealthChecksDashboard/nuget.config", "health-checks-dashboard/src/Nexus.HealthChecksDashboard/"]
COPY ["health-checks-dashboard/src/Nexus.HealthChecksDashboard/Nexus.HealthChecksDashboard.csproj", "health-checks-dashboard/src/Nexus.HealthChecksDashboard/"]

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "health-checks-dashboard/src/Nexus.HealthChecksDashboard/Nexus.HealthChecksDashboard.csproj"

COPY . .

WORKDIR "/src/health-checks-dashboard/src/Nexus.HealthChecksDashboard"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build "Nexus.HealthChecksDashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "Nexus.HealthChecksDashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.HealthChecksDashboard.dll"]
