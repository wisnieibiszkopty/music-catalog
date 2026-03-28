FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH

RUN apk add --no-cache clang lld musl-dev build-base zlib-dev

WORKDIR /src
COPY ["src/gateway/ApiGateway/ApiGateway.csproj", "src/gateway/ApiGateway/"]

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet restore "src/gateway/ApiGateway/ApiGateway.csproj" \
    -r linux-musl-$TARGETARCH

COPY . .
WORKDIR "/src/src/gateway/ApiGateway"

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet build "./ApiGateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore \
    -r linux-musl-$TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN --mount=type=cache,id=nuget-$TARGETARCH,target=/root/.nuget/packages \
    dotnet publish "./ApiGateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    -r linux-musl-$TARGETARCH \
    /p:PublishAot=true \
    /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./ApiGateway"]