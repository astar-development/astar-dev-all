FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/console-apps/AStar.Dev.Database.Updater/AStar.Dev.Database.Updater.csproj", "src/console-apps/AStar.Dev.Database.Updater/"]
COPY ["src/nuget-packages/AStar.Dev.Files.Api.Client.SDK/AStar.Dev.Files.Api.Client.SDK.csproj", "src/nuget-packages/AStar.Dev.Files.Api.Client.SDK/"]
COPY ["src/nuget-packages/AStar.Dev.Api.Client.Sdk.Shared/AStar.Dev.Api.Client.Sdk.Shared.csproj", "src/nuget-packages/AStar.Dev.Api.Client.Sdk.Shared/"]
COPY ["src/nuget-packages/AStar.Dev.Api.HealthChecks/AStar.Dev.Api.HealthChecks.csproj", "src/nuget-packages/AStar.Dev.Api.HealthChecks/"]
COPY ["src/nuget-packages/AStar.Dev.Api.Usage.Sdk/AStar.Dev.Api.Usage.Sdk.csproj", "src/nuget-packages/AStar.Dev.Api.Usage.Sdk/"]
COPY ["src/nuget-packages/AStar.Dev.Utilities/AStar.Dev.Utilities.csproj", "src/nuget-packages/AStar.Dev.Utilities/"]
COPY ["src/nuget-packages/AStar.Dev.AspNet.Extensions/AStar.Dev.AspNet.Extensions.csproj", "src/nuget-packages/AStar.Dev.AspNet.Extensions/"]
COPY ["src/nuget-packages/AStar.Dev.Technical.Debt.Reporting/AStar.Dev.Technical.Debt.Reporting.csproj", "src/nuget-packages/AStar.Dev.Technical.Debt.Reporting/"]
COPY ["src/nuget-packages/AStar.Dev.Infrastructure.FilesDb/AStar.Dev.Infrastructure.FilesDb.csproj", "src/nuget-packages/AStar.Dev.Infrastructure.FilesDb/"]
COPY ["src/nuget-packages/AStar.Dev.Infrastructure/AStar.Dev.Infrastructure.csproj", "src/nuget-packages/AStar.Dev.Infrastructure/"]
COPY ["src/nuget-packages/AStar.Dev.Logging.Extensions/AStar.Dev.Logging.Extensions.csproj", "src/nuget-packages/AStar.Dev.Logging.Extensions/"]
RUN dotnet restore "src/console-apps/AStar.Dev.Database.Updater/AStar.Dev.Database.Updater.csproj"
COPY . .
WORKDIR "/src/src/console-apps/AStar.Dev.Database.Updater"
RUN dotnet build "AStar.Dev.Database.Updater.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AStar.Dev.Database.Updater.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AStar.Dev.Database.Updater.dll"]
