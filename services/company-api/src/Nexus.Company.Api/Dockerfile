FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["services/company-api/src/Nexus.Company.Api/nuget.config", "services/company-api/src/Nexus.Company.Api/"]
COPY ["services/company-api/src/Nexus.Company.Api/Nexus.Company.Api.csproj", "services/company-api/src/Nexus.Company.Api/"]

RUN dotnet restore "services/company-api/src/Nexus.Company.Api/Nexus.Company.Api.csproj"

COPY . .

WORKDIR "/src/services/company-api/src/Nexus.Company.Api"
RUN dotnet build "Nexus.Company.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexus.Company.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.Company.Api.dll"]
