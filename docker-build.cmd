@ECHO OFF
CLS
ECHO.
SET IMAGE_ID=sd-feeds-share
SET IMAGE_DESCRIPTION=SelfDrvn Feed Share

IF "%~1"=="--tag" (GOTO VERSION) ELSE GOTO UNKNOWN

:VERSION
IF "%2"=="" GOTO UNKNOWN

ECHO.
ECHO Building %IMAGE_DESCRIPTION% Docker Image: %IMAGE_ID%:%2
docker build -t %IMAGE_ID%:%2 .
GOTO DONE

:UNKNOWN
ECHO Usage:
ECHO docker-build.cmd --tag [tag]
ECHO.

:DONE