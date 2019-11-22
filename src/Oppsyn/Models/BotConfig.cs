namespace Oppsyn.Models
{
    public class BotConfig
    {
        public SlackConfig Slack { get; set; }
        public AzureConfig Azure { get; set; }
    }

    public class AzureConfig
    {
        public string ServiceToken { get; set; }
        public string Endpoint { get; set; }
    }

    public class SlackConfig
    {
        /// <summary>
        /// Has access to rtm api and general user access as bot
        /// </summary>
        public string BotUserToken { get; set; }
        /// <summary>
        /// Can be given different accesses, inclusding seeing full channel history and reading old messages.
        /// </summary>
        public string OauthAccessToken { get; set; }
        public bool PostFindings { get; internal set; }
    }
}