FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/apis/AStar.Dev.File.Classifications.Api/AStar.Dev.File.Classifications.Api.csproj", "src/apis/AStar.Dev.File.Classifications.Api/"]
RUN dotnet restore "src/apis/AStar.Dev.File.Classifications.Api/AStar.Dev.File.Classifications.Api.csproj"
COPY . .
WORKDIR "/src/src/apis/AStar.Dev.File.Classifications.Api"
RUN dotnet build "./AStar.Dev.File.Classifications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AStar.Dev.File.Classifications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AStar.Dev.File.Classifications.Api.dll"]
