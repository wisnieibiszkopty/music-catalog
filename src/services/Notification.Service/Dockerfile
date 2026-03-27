FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/services/Notification.Service/Notification.Service.csproj", "src/services/Notification.Service/"]
COPY ["src/shared/Shared/Shared.csproj", "src/shared/Shared/"]
COPY ["src/shared/Contracts/Contracts.csproj", "src/shared/Contracts/"]

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "src/services/Notification.Service/Notification.Service.csproj"
    
COPY . .
WORKDIR "/src/src/services/Notification.Service"

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build "./Notification.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notification.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Service.dll"]


