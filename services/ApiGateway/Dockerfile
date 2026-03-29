FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

RUN apk add --no-cache clang lld musl-dev build-base zlib-dev

WORKDIR /src

COPY ["services/ApiGateway/ApiGateway.csproj", "services/ApiGateway/"]
COPY ["shared/Shared/Shared.csproj", "shared/Shared/"]
COPY ["shared/Contracts/Contracts.csproj", "shared/Contracts/"]

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet restore "services/ApiGateway/ApiGateway.csproj" \
    -r linux-musl-$TARGETARCH

COPY services/ApiGateway/ services/ApiGateway/
COPY shared/ shared/

WORKDIR "/src/services/ApiGateway"

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet build ./ -c $BUILD_CONFIGURATION -o /app/build --no-restore -r linux-musl-$TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet publish ./ -c $BUILD_CONFIGURATION -o /app/publish --no-restore \
    -r linux-musl-$TARGETARCH \
    /p:PublishAot=true \
    /p:UseAppHost=true

FROM base AS final
WORKDIR /app

LABEL \
    org.opencontainers.image.title="api-gateway" \
    org.opencontainers.image.description="Api gateway" \
    org.opencontainers.image.source="https://github.com/wisnieibiszkopty/music-catalog"

COPY --from=publish /app/publish .
ENTRYPOINT ["./ApiGateway"]