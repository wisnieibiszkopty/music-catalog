#!/bin/bash

set -e

DOCKERHUB_USER="wisnieibiszkopty"
TAG="0.1.0"
PLATFORMS="linux/amd64,linux/arm64"

cd "$(dirname "$0")/.."
echo "Workdir: $(pwd)"

echo "Logging to Docker Hub... "
docker login

echo "Configuring multiarchitecture build"
docker buildx create --name multiarch-builder --use 2>/dev/null || docker buildx use multiarch-builder
docker buildx inspect --bootstrap

build_service(){
  local IMAGE_NAME=$1
  local DOCKERFILE_PATH=$2
  
  echo "Building image: $IMAGE_NAME"
  
  docker buildx build \
    --platform "$PLATFORMS" \
    -t "${DOCKERHUB_USER}/music-catalog-${IMAGE_NAME}:${TAG}" \
    -f "${DOCKERFILE_PATH}" \
    --no-cache \
    --sbom=true \
    --provenance=mode=max \
    --push \
    .
}

build_api_gateway_multiarch(){
  local IMAGE_NAME="api-gateway"

  docker buildx build \
    --platform linux/amd64 \
    -t "${DOCKERHUB_USER}/music-catalog-${IMAGE_NAME}:${TAG}-amd64" \
    -f "services/ApiGateway/Dockerfile" \
    --no-cache \
    --sbom=true \
    --provenance=mode=max \
    --push \
    .

  docker buildx build \
    --platform linux/arm64 \
    -t "${DOCKERHUB_USER}/music-catalog-${IMAGE_NAME}:${TAG}-arm64" \
    -f "services/ApiGateway/Dockerfile_arm" \
    --no-cache \
    --sbom=true \
    --provenance=mode=max \
    --push \
    .

  docker buildx imagetools create \
    -t "${DOCKERHUB_USER}/music-catalog-${IMAGE_NAME}:${TAG}" \
    "${DOCKERHUB_USER}/music-catalog-${IMAGE_NAME}:${TAG}-amd64" \
    "${DOCKERHUB_USER}/music-catalog-${IMAGE_NAME}:${TAG}-arm64"
}

build_service "web-app" "frontend/Dockerfile"

build_api_gateway_multiarch
build_service "artist-service" "services/Artists.Service/Dockerfile"
build_service "catalog-service" "services/Catalog.Service/Dockerfile"
build_service "notification-service" "services/Notification.Service/Dockerfile"
build_service "orchestrator-service" "services/Orchestrator.Service/Dockerfile"
build_service "scraper-service" "services/Scraper.Service/Dockerfile"

echo "All images were successfully built"