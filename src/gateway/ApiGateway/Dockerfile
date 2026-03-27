FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN apk add --no-cache clang lld musl-dev build-base zlib-dev

COPY ["src/gateway/ApiGateway/ApiGateway.csproj", "src/gateway/ApiGateway/"]

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "src/gateway/ApiGateway/ApiGateway.csproj" -r linux-musl-x64

COPY . .
WORKDIR "/src/src/gateway/ApiGateway"

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "./ApiGateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    -r linux-musl-x64 \
    --no-restore \
    /p:PublishAot=true \
    /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./ApiGateway"]