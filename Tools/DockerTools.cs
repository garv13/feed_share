using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using SelfDrvn.Feeds.Share.Configs;

namespace SelfDrvn.Feeds.Share.Tools
{
    public static class DockerTools
    {
        private const string IDP_AUTHORITY = "IDP_AUTHORITY";
        private const string IDP_CLIENT_ID = "IDP_CLIENT_ID";
        private const string IDP_CLIENT_SECRET = "IDP_CLIENT_SECRET";
        private const string DATABASE_SERVER = "DATABASE_SERVER";
        private const string DATABASE_PORT = "DATABASE_PORT";
        private const string DATABASE_NAME = "DATABASE_NAME";
        private const string DATABASE_USER = "DATABASE_USER";
        private const string DATABASE_PASSWORD = "DATABASE_PASSWORD";
        private const string DATABASE_PARAMS = "DATABASE_PARAMS";
        private const string CONNECTION_STRING = "CONNECTION_STRING";
        private const string FACEBOOK_APP_ID = "FACEBOOK_APP_ID";
        private const string USER_ID_FORMAT = "USER_ID_FORMAT";

        public static AppSettings ApplyDockerEnvironment(this AppSettings appSettings)
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(IDP_AUTHORITY)))
                appSettings.IdentityClientSetting.IdentityAuthority =
                    Environment.GetEnvironmentVariable(IDP_AUTHORITY);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(IDP_CLIENT_ID)))
                appSettings.IdentityClientSetting.IdentityClientId =
                    Environment.GetEnvironmentVariable(IDP_CLIENT_ID);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(IDP_CLIENT_SECRET)))
                appSettings.IdentityClientSetting.IdentityClientSecret =
                    Environment.GetEnvironmentVariable(IDP_CLIENT_SECRET);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(FACEBOOK_APP_ID)))
                appSettings.SocialMediaSettings.FacebookAppID =
                    Environment.GetEnvironmentVariable(FACEBOOK_APP_ID);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(USER_ID_FORMAT)))
                appSettings.UserIdFormat =
                    Environment.GetEnvironmentVariable(USER_ID_FORMAT);
            return appSettings;
        }

        public static string UseDockerizeConnectionString(AppSettings appSettings)
        {
            var connectionString = Environment.GetEnvironmentVariable(CONNECTION_STRING);
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            var server = Environment.GetEnvironmentVariable(DATABASE_SERVER);
            var port = Environment.GetEnvironmentVariable(DATABASE_PORT);
            var user = Environment.GetEnvironmentVariable(DATABASE_USER);
            var password = Environment.GetEnvironmentVariable(DATABASE_PASSWORD);
            var parameters = Environment.GetEnvironmentVariable(DATABASE_PARAMS);

            if (!string.IsNullOrEmpty(server))
            {
                var sb = new StringBuilder("mongodb://");
                if (!string.IsNullOrEmpty(user))
                    sb.Append(user);
                if (!string.IsNullOrEmpty(password))
                    sb.Append($":{password}");
                sb.Append($"@{server}");
                if (!string.IsNullOrEmpty(port))
                    sb.Append($":{port}");
                if (!string.IsNullOrEmpty(parameters))
                    sb.Append($"/?{parameters}");
                return sb.ToString();
            }

            return appSettings.ConnectionString;
        }

        public static string UseDockerizeDatabaseName(AppSettings appSettings)
        {
            var database = Environment.GetEnvironmentVariable(DATABASE_NAME);
            if (!string.IsNullOrEmpty(database))
            {
                return database;
            }
            return appSettings.Database;
        }
    }
}