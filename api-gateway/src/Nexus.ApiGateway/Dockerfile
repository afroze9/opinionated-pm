FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["appsettings.Global.json", "appsettings.Global.json"]
COPY ["api-gateway/src/Nexus.ApiGateway/nuget.config", "api-gateway/src/Nexus.ApiGateway/"]
COPY ["api-gateway/src/Nexus.ApiGateway/Nexus.ApiGateway.csproj", "api-gateway/src/Nexus.ApiGateway/"]

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "api-gateway/src/Nexus.ApiGateway/Nexus.ApiGateway.csproj"

COPY . .

WORKDIR "/src/api-gateway/src/Nexus.ApiGateway"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build "Nexus.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "Nexus.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.ApiGateway.dll"]
