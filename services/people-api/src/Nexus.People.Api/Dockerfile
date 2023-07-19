FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["services/people-api/src/Nexus.People.Api/nuget.config", "services/people-api/src/Nexus.People.Api/"]
COPY ["services/people-api/src/Nexus.People.Api/Nexus.People.Api.csproj", "services/people-api/src/Nexus.People.Api/"]

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "services/people-api/src/Nexus.People.Api/Nexus.People.Api.csproj"

COPY ["appsettings.Global.json", "appsettings.Global.json"]
COPY . .

WORKDIR "/src/services/people-api/src/Nexus.People.Api"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build "Nexus.People.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "Nexus.People.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.People.Api.dll"]
