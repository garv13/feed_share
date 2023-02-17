#!/bin/sh
IMAGE_ID="sd-feeds-share"
IMAGE_DESCRIPTION="SelfDrvn Feed Share"

if [ -z "$1" ] || [ -z "$2"  ]; then
    echo "Usage:"
    echo "docker-build.cmd --tag [tag]"
    exit 1
fi

echo "Building $IMAGE_DESCRIPTION Docker Image: $IMAGE_ID:$2"
docker build -t $IMAGE_ID:$2 .

exit 0