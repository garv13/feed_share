FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out SelfDrvn.Feeds.Share.csproj

# Local configuration cleanup
RUN rm ./out/appsettings.json
COPY ./appsettings.Docker.json ./out/appsettings.json
RUN rm ./out/appsettings.*.json

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:2.1
WORKDIR /app

# Configure Application
ENV DATABASE_SERVER=""\
    DATABASE_PORT=""\
    DATABASE_NAME=""\
    DATABASE_USER=""\
    DATABASE_PASSWORD=""\
    DATABASE_PARAMS=""\
    IDP_AUTHORITY=""\
    IDP_CLIENT_ID=""\
    IDP_CLIENT_SECRET=""\
    CONNECTION_STRING=""\
    FACEBOOK_APP_ID=""\
    USER_ID_FORMAT=""

# Copy binary from build image
COPY --from=build-env /app/out .

# Exposing Webserver
EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "SelfDrvn.Feeds.Share.dll"]