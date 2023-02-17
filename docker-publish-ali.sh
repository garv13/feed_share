#!/bin/bash
IMAGE_ID="sd-feeds-share"
IMAGE_DESCRIPTION="SelfDrvn Feed Share"

if [ -z "$1" ] || [ -z "$2"  ]; then
    echo "Usage:"
    echo "docker-publish-ali.cmd --tag [tag]"
    exit 1
fi

echo "Publishing $IMAGE_DESCRIPTION Docker Image: $IMAGE_ID:$2"
docker tag $IMAGE_ID:$2 registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/$IMAGE_ID:$2
docker tag $IMAGE_ID:$2 registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/$IMAGE_ID:latest
docker push registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/$IMAGE_ID:$2
docker push registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/$IMAGE_ID:latest

exit 0