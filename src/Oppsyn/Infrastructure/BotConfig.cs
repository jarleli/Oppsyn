namespace Oppsyn
{
    public class BotConfig
    {
        public string SlackApiKey { get; set; }
        public string AzureServiceToken { get; set; }
        public string AzureServiceEndpoint { get; set; }
        public bool PostFindings { get; internal set; }
    }
}