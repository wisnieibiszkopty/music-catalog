FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

WORKDIR /src

COPY ["src/services/Orchestrator.Service/Orchestrator.Service.csproj", "src/services/Orchestrator.Service/"]
COPY ["src/shared/Shared/Shared.csproj", "src/shared/Shared/"]
COPY ["src/shared/Contracts/Contracts.csproj", "src/shared/Contracts/"]

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet restore "src/services/Orchestrator.Service/Orchestrator.Service.csproj" -a $TARGETARCH
    
COPY . .
WORKDIR "/src/src/services/Orchestrator.Service"

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet build "./Orchestrator.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore -a $TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orchestrator.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -a $TARGETARCH

FROM base AS final
WORKDIR /app

LABEL \
    org.opencontainers.image.title="orchestrator-service" \
    org.opencontainers.image.description="Orchestrator microservice" \
    org.opencontainers.image.source="https://github.com/wisnieibiszkopty/music-catalog"

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orchestrator.Service.dll"]
