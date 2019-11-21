using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



namespace Oppsyn.SlackClients
{
    public partial class FullIncomingMessage
    {
        [JsonProperty("client_msg_id")]
        public Guid ClientMsgId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }
        
        [JsonProperty("thread_ts", NullValueHandling = NullValueHandling.Ignore)]
        public string ThreadTs { get;  set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; } = new Attachment[0];

        [JsonProperty("blocks")]
        public Block[] Blocks { get; set; } = new Block[0];
    }

    public partial class Attachment
    {
        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_link")]
        public Uri TitleLink { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("thumb_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ThumbUrl { get; set; }

        [JsonProperty("from_url")]
        public Uri FromUrl { get; set; }

        [JsonProperty("thumb_width", NullValueHandling = NullValueHandling.Ignore)]
        public long? ThumbWidth { get; set; }

        [JsonProperty("thumb_height", NullValueHandling = NullValueHandling.Ignore)]
        public long? ThumbHeight { get; set; }

        [JsonProperty("service_icon")]
        public Uri ServiceIcon { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("original_url")]
        public Uri OriginalUrl { get; set; }

        [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ImageUrl { get; set; }

        [JsonProperty("ts", NullValueHandling = NullValueHandling.Ignore)]
        public long? Ts { get; set; }

        [JsonProperty("image_width", NullValueHandling = NullValueHandling.Ignore)]
        public long? ImageWidth { get; set; }

        [JsonProperty("image_height", NullValueHandling = NullValueHandling.Ignore)]
        public long? ImageHeight { get; set; }

        [JsonProperty("image_bytes", NullValueHandling = NullValueHandling.Ignore)]
        public long? ImageBytes { get; set; }
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

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }
}