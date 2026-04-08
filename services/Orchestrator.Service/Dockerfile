FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

WORKDIR /src

COPY ["services/Orchestrator.Service/Orchestrator.Service.csproj", "services/Orchestrator.Service/"]
COPY ["shared/Shared/Shared.csproj", "shared/Shared/"]
COPY ["shared/Contracts/Contracts.csproj", "shared/Contracts/"]

RUN --mount=type=cache,id=nuget-Orchestrator-$TARGETARCH,target=/root/.nuget/packages \
    dotnet restore "services/Orchestrator.Service/Orchestrator.Service.csproj" -a $TARGETARCH

COPY services/Orchestrator.Service/ services/Orchestrator.Service/
COPY shared/ shared/

WORKDIR "/src/services/Orchestrator.Service"
RUN --mount=type=cache,id=nuget-Orchestrator-$TARGETARCH,target=/root/.nuget/packages \
    dotnet build ./ -c $BUILD_CONFIGURATION -o /app/build --no-restore -a $TARGETARCH

FROM build AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,id=nuget-Orchestrator-$TARGETARCH,target=/root/.nuget/packages \
    dotnet publish "Orchestrator.Service.csproj" -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

LABEL \
    org.opencontainers.image.title="orchestrator-service" \
    org.opencontainers.image.version="0.1.0" \
    org.opencontainers.image.description="Orchestrator microservice" \
    org.opencontainers.image.source="https://github.com/wisnieibiszkopty/music-catalog"

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orchestrator.Service.dll"]