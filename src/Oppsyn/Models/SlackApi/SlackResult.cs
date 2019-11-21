using Newtonsoft.Json;

namespace Oppsyn.SlackClients
{
    public class SlackResult<T>
    {
        [JsonProperty("OK")]
        public bool Ok{ get; set; }
        [JsonProperty("latest")]
        public string Latest{ get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("messages")]
        public T[] Messages { get; set; }

        //"has_more": true,
        //"pin_count": 0,
        //"channel_actions_ts": null,
        //"channel_actions_count": 0,
        //"response_metadata": {
        //    "next_cursor": "bmV4dF90czoxNTc0MzczNjcxMDA1OTAw"

    }
}