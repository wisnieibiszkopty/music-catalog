FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS base
WORKDIR /app

RUN apk add --no-cache icu-libs libintl bash curl
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release

RUN apk add --no-cache clang build-base zlib-dev

WORKDIR /src

COPY ["src/services/Orchestrator.Service/Orchestrator.Service.csproj", "src/services/Orchestrator.Service/"]
COPY ["src/shared/Contracts/Contracts.csproj", "src/shared/Contracts/"]
COPY ["src/shared/Shared/Shared.csproj", "src/shared/Shared/"]

RUN dotnet restore "src/services/Orchestrator.Service/Orchestrator.Service.csproj"

COPY . .
WORKDIR "/src/src/services/Orchestrator.Service"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orchestrator.Service.csproj" \
    -c $BUILD_CONFIGURATION \
    -r linux-musl-x64 \
    -o /app/publish \
    /p:PublishAot=true \
    /p:StripSymbols=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN chmod +x ./Orchestrator.Service

USER $APP_UID

ENTRYPOINT ["./Orchestrator.Service"]