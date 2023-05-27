FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Nexus.Company.Api/Nexus.Company.Api.csproj", "./"]
COPY ["src/Nexus.Company.Api/nuget.config", "./"]
RUN dotnet restore --configfile nuget.config "Nexus.Company.Api.csproj"

COPY ["src/Nexus.Company.Api/*", "./"]
RUN dotnet build -c Release -o /app/build --no-restore "Nexus.Company.Api.csproj"

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false --no-restore "Nexus.Company.Api.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.Company.Api.dll"]
