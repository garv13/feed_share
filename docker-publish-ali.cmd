@ECHO OFF
CLS
ECHO.
SET IMAGE_ID=sd-feeds-share
SET IMAGE_DESCRIPTION=SelfDrvn Feed Share

IF "%~1"=="--tag" (GOTO VERSION) ELSE GOTO UNKNOWN

:VERSION
IF "%2"=="" GOTO UNKNOWN

ECHO.
ECHO Publishing %IMAGE_DESCRIPTION% Docker Image: %IMAGE_ID%:%2
docker tag %IMAGE_ID%:%2 registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/%IMAGE_ID%:%2
docker tag %IMAGE_ID%:%2 registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/%IMAGE_ID%:latest
docker push registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/%IMAGE_ID%:%2
docker push registry-intl.ap-southeast-3.aliyuncs.com/selfdrvn/%IMAGE_ID%:latest
GOTO DONE

:UNKNOWN
ECHO Usage:
ECHO docker-publish-ali.cmd --tag [tag]
ECHO.

:DONE