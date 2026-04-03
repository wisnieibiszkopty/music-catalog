FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

WORKDIR /src

COPY ["services/Catalog.Service/Catalog.Service.csproj", "services/Catalog.Service/"]
COPY ["shared/Shared/Shared.csproj", "shared/Shared/"]
COPY ["shared/Contracts/Contracts.csproj", "shared/Contracts/"]

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet restore "services/Catalog.Service/Catalog.Service.csproj" -a $TARGETARCH
    
COPY services/Catalog.Service/ services/Catalog.Service/
COPY shared/ shared/

WORKDIR "/src/services/Catalog.Service"
RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet build ./ -c $BUILD_CONFIGURATION -o /app/build --no-restore -a $TARGETARCH

FROM build AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet publish "Catalog.Service.csproj" -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

LABEL \
    org.opencontainers.image.title="catalog-service" \
    org.opencontainers.image.version="0.1.0" \
    org.opencontainers.image.description="Catalog microservice" \
    org.opencontainers.image.source="https://github.com/wisnieibiszkopty/music-catalog"

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Service.dll"]
