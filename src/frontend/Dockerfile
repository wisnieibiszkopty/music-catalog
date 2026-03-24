FROM node:22-alpine AS build-stage

ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

RUN npm install -g pnpm

WORKDIR /app

COPY package.json pnpm-lock.yaml ./

RUN pnpm install --frozen-lockfile

COPY . .

RUN VITE_API_URL=$VITE_API_URL npm run build

FROM joseluisq/static-web-server:2.21-alpine AS production-stage

WORKDIR /public

COPY --from=build-stage /app/dist .

ENV SERVER_PORT=80
ENV SERVER_ROOT=/public
ENV SERVER_ERROR_404=/public/index.html
ENV SERVER_SECURITY_HEADERS=true

USER 1000

EXPOSE 80