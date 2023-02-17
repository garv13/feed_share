namespace SelfDrvn.Feeds.Share.Configs
{
    public class AppSettings
    {
        public bool AgentEnabled { get; set; }
        public IdentityClientSetting IdentityClientSetting { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string UserIdFormat { get; set; }    // allowed values: email, userid
        public SocialMediaSettings SocialMediaSettings { get; set; }
    }
}