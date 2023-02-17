using SelfDrvn.Feeds.Share.Configs;

namespace SelfDrvn.Feeds.Share.Services
{
    public interface IAppSettingsService
    {
        AppSettings Settings { get; }
    }

    public class AppSettingsService : IAppSettingsService
    {
        public AppSettings Settings { get; }

        public AppSettingsService(AppSettings settings)
        {
            Settings = settings;
        }
    }
}
