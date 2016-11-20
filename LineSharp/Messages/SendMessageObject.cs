using System.Collections.Generic;
using Newtonsoft.Json;

namespace LIneSharp.Messages
{
    /// <summary>
    /// https://devdocs.line.me/#reply-message
    /// </summary>
    public class ReplyMessage
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("messages")]
        public IEnumerable<SendMessageObject> Messages { get; set; }
    }

    /// <summary>
    /// https://devdocs.line.me/#push-message
    /// </summary>
    public class PushMessage
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("messages")]
        public IEnumerable<SendMessageObject> Messages { get; set; }
    }

    /// <summary>
    /// https://devdocs.line.me/#send-message-object
    /// </summary>
    public class SendMessageObject
    {
        // common
        [JsonProperty("type")]
        public string Type { get; set; }

        // text
        [JsonProperty("text")]
        public string Text { get; set; }

        // image, video, audio
        [JsonProperty("originalContentUrl")]
        public string OriginalContentUrl { get; set; }

        // image, video
        [JsonProperty("previewIageUrl")]
        public string PreviewImageUrl { get; set; }

        // audio
        [JsonProperty("duration")]
        public double Duration { get; set; }

        // location
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }

        // sticker
        [JsonProperty("packageId")]
        public string PackageId { get; set; }

        [JsonProperty("stickerId")]
        public string StickerId { get; set; }
    }
}
