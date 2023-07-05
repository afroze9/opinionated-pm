FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["services/project-api/src/Nexus.Project.Api/nuget.config", "services/project-api/src/Nexus.Project.Api/"]
COPY ["services/project-api/src/Nexus.Project.Api/Nexus.Project.Api.csproj", "services/project-api/src/Nexus.Project.Api/"]

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "services/project-api/src/Nexus.Project.Api/Nexus.Project.Api.csproj"

COPY . .

WORKDIR "/src/services/project-api/src/Nexus.Project.Api"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build "Nexus.Project.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "Nexus.Project.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.Project.Api.dll"]