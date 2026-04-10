#!/bin/bash

REPORT_DIR="reports"
mkdir -p $REPORT_DIR

COMPOSE_FILE="docker-compose.yml"

if ! command -v yq &> /dev/null; then
    echo "Please install yq :'((("
    exit 1
fi

# getting unique images from compose file
IMAGES=$(yq -r '.services[].image' "$COMPOSE_FILE" | grep -v 'null' | sort -u)

for IMG in $IMAGES
do
  BASE_NAME=$(echo $IMG | sed 's/[\/:]/_/g')
  TABLE_REPORT="$REPORT_DIR/${BASE_NAME}.html"
  
  echo "Scanning $IMG"

  trivy image --format template --template "@html.tpl" -o "$TABLE_REPORT" "$IMG"
  
done

echo "Finished"