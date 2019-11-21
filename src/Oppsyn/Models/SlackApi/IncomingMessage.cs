namespace Oppsyn.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class IncomingMessage
    {
        [JsonProperty("client_msg_id")]
        public Guid ClientMsgId { get; set; }

        [JsonProperty("suppress_notification")]
        public bool SuppressNotification { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("blocks")]
        public Block[] Blocks { get; set; }

        [JsonProperty("user_team")]
        public string UserTeam { get; set; }

        [JsonProperty("source_team")]
        public string SourceTeam { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("event_ts")]
        public string EventTs { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }
    }

    public partial class Block
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }

        [JsonProperty("elements")]
        public BlockElement[] Elements { get; set; }
    }

    public partial class BlockElement
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("elements")]
        public ElementElementClass[] Elements { get; set; }
    }

    public partial class ElementElementClass
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}