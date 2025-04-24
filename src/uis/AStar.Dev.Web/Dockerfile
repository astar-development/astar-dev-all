FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/uis/AStar.Dev.Web/AStar.Dev.Web.csproj", "src/uis/AStar.Dev.Web/"]
COPY ["src/nuget-packages/AStar.Dev.Admin.Api.Client.Sdk/AStar.Dev.Admin.Api.Client.Sdk.csproj", "src/nuget-packages/AStar.Dev.Admin.Api.Client.Sdk/"]
COPY ["src/nuget-packages/AStar.Dev.Api.Client.Sdk.Shared/AStar.Dev.Api.Client.Sdk.Shared.csproj", "src/nuget-packages/AStar.Dev.Api.Client.Sdk.Shared/"]
COPY ["src/nuget-packages/AStar.Dev.Api.HealthChecks/AStar.Dev.Api.HealthChecks.csproj", "src/nuget-packages/AStar.Dev.Api.HealthChecks/"]
COPY ["src/nuget-packages/AStar.Dev.Api.Usage.Sdk/AStar.Dev.Api.Usage.Sdk.csproj", "src/nuget-packages/AStar.Dev.Api.Usage.Sdk/"]
COPY ["src/nuget-packages/AStar.Dev.Utilities/AStar.Dev.Utilities.csproj", "src/nuget-packages/AStar.Dev.Utilities/"]
COPY ["src/nuget-packages/AStar.Dev.AspNet.Extensions/AStar.Dev.AspNet.Extensions.csproj", "src/nuget-packages/AStar.Dev.AspNet.Extensions/"]
COPY ["src/nuget-packages/AStar.Dev.Technical.Debt.Reporting/AStar.Dev.Technical.Debt.Reporting.csproj", "src/nuget-packages/AStar.Dev.Technical.Debt.Reporting/"]
COPY ["src/nuget-packages/AStar.Dev.Files.Api.Client.SDK/AStar.Dev.Files.Api.Client.SDK.csproj", "src/nuget-packages/AStar.Dev.Files.Api.Client.SDK/"]
COPY ["src/nuget-packages/AStar.Dev.Images.Api.Client.SDK/AStar.Dev.Images.Api.Client.SDK.csproj", "src/nuget-packages/AStar.Dev.Images.Api.Client.SDK/"]
COPY ["src/nuget-packages/AStar.Dev.Logging.Extensions/AStar.Dev.Logging.Extensions.csproj", "src/nuget-packages/AStar.Dev.Logging.Extensions/"]
RUN dotnet restore "src/uis/AStar.Dev.Web/AStar.Dev.Web.csproj"
COPY . .
WORKDIR "/src/src/uis/AStar.Dev.Web"
RUN dotnet build "./AStar.Dev.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AStar.Dev.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AStar.Dev.Web.dll"]
