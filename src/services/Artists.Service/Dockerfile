FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

RUN apk add --no-cache \
    clang \
    build-base \
    musl-dev \
    zlib-dev

WORKDIR /src

COPY . .

RUN dotnet publish src/services/Artists.Service/Artists.Service.csproj \
    -c Release \
    -r linux-musl-x64 \
    -p:PublishAot=true \
    -p:SelfContained=true \
    -p:StripSymbols=true \
    -o /app

FROM alpine:3.20

RUN adduser -D appuser
USER appuser

WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["./Artists.Service"]