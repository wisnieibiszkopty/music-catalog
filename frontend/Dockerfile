FROM node:22-alpine AS build-stage

RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

COPY ["frontend/package.json", "frontend/pnpm-lock.yaml", "./"]

RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --frozen-lockfile

COPY frontend/ .

ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

RUN pnpm run build

FROM joseluisq/static-web-server:2.21-alpine AS production-stage

WORKDIR /public

COPY --from=build-stage /app/dist .

ENV SERVER_PORT=80 \
    SERVER_ROOT=/public \
    SERVER_FALLBACK_PAGE=index.html \
    SERVER_SECURITY_HEADERS=true \
    SERVER_COMPRESSION=true \
    SERVER_LOG_LEVEL=info
    
USER 1000

EXPOSE 80