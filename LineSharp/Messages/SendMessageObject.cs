using System.Collections.Generic;
using LineSharp.Json;
using Newtonsoft.Json;

namespace LineSharp.Messages
{
    /// <summary>
    /// https://devdocs.line.me/#reply-message
    /// </summary>
    public class ReplyMessage
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("messages")]
        public IEnumerable<SendMessageBase> Messages { get; set; }
    }

    /// <summary>
    /// https://devdocs.line.me/#push-message
    /// </summary>
    public class PushMessage
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("messages")]
        public IEnumerable<SendMessageBase> Messages { get; set; }
    }

    /// <summary>
    /// https://devdocs.line.me/#send-message-object
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes))]
    public class SendMessageBase
    {
        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    [JsonSubtype(MessageType.Text)]
    public class TextMessage : SendMessageBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    [JsonSubtype(MessageType.Image)]
    public class ImageMessage : SendMessageBase
    {
        [JsonProperty("originalContentUrl")]
        public string OriginalContentUrl { get; set; }

        [JsonProperty("previewIageUrl")]
        public string PreviewImageUrl { get; set; }
    }

    [JsonSubtype(MessageType.Video)]
    public class VideoMessage : SendMessageBase
    {
        [JsonProperty("originalContentUrl")]
        public string OriginalContentUrl { get; set; }

        [JsonProperty("previewIageUrl")]
        public string PreviewImageUrl { get; set; }
    }

    [JsonSubtype(MessageType.Audio)]
    public class AudioMessage : SendMessageBase
    {
        [JsonProperty("originalContentUrl")]
        public string OriginalContentUrl { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }
    }

    [JsonSubtype(MessageType.Location)]
    public class LocationMessage : SendMessageBase
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
    }

    [JsonSubtype(MessageType.Sticker)]
    public class StickerMessage : SendMessageBase
    {
        [JsonProperty("packageId")]
        public string PackageId { get; set; }

        [JsonProperty("stickerId")]
        public string StickerId { get; set; }
    }
}
