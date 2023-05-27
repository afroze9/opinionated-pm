FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Nexus.ApiGateway/Nexus.ApiGateway.csproj", "./"]
COPY ["src/Nexus.ApiGateway/nuget.config", "./"]
RUN dotnet restore --configfile nuget.config "Nexus.ApiGateway.csproj"

COPY ["src/Nexus.ApiGateway/*", "./"]
RUN dotnet build -c Release -o /app/build --no-restore "Nexus.ApiGateway.csproj"

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false --no-restore "Nexus.ApiGateway.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY src/Nexus.ApiGateway/Ocelot /app/Ocelot
ENTRYPOINT ["dotnet", "Nexus.ApiGateway.dll"]
