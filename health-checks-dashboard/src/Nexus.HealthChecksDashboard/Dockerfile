FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Nexus.HealthChecksDashboard/Nexus.HealthChecksDashboard.csproj", "src/Nexus.HealthChecksDashboard/"]
RUN dotnet restore "src/Nexus.HealthChecksDashboard/Nexus.HealthChecksDashboard.csproj"
COPY . .
WORKDIR "/src/src/Nexus.HealthChecksDashboard"
RUN dotnet build "Nexus.HealthChecksDashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexus.HealthChecksDashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.HealthChecksDashboard.dll"]
