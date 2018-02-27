using System.Collections.Generic;
using System.Runtime.Serialization;
using LineSharp.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [JsonProperty("previewImageUrl")]
        public string PreviewImageUrl { get; set; }
    }

    [JsonSubtype(MessageType.Video)]
    public class VideoMessage : SendMessageBase
    {
        [JsonProperty("originalContentUrl")]
        public string OriginalContentUrl { get; set; }

        [JsonProperty("previewImageUrl")]
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

    [JsonSubtype(MessageType.Imagemap)]
    public class ImagemapMessage : SendMessageBase
    {
        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonProperty("altText")]
        public string AltText { get; set; }

        [JsonProperty("baseSize")]
        public Size BaseSize { get; set; } = new Size();

        [JsonProperty("actions")]
        public IEnumerable<ImagemapActionBase> Actions { get; set; }
    }

    public class Size
    {
        [JsonProperty("width")]
        public int Width { get; set; } = 1040;

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes))]
    public class ImagemapActionBase
    {
        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("area")]
        public ImagemapAreaObject Area { get; set; }
    }

    [JsonSubtype("uri")]
    public class UriAction : ImagemapActionBase
    {
        [JsonProperty("linkUri")]
        public string LinkUri { get; set; }
    }

    [JsonSubtype("message")]
    public class MessageAction : ImagemapActionBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class ImagemapAreaObject
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    [JsonSubtype(MessageType.Template)]
    public class TemplateMessage : SendMessageBase
    {
        [JsonProperty("altText")]
        public string AltText { get; set; }

        [JsonProperty("template")]
        public TemplateObjectBase Template { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes))]
    public class TemplateObjectBase
    {
        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    [JsonSubtype("buttons")]
    public class ButtonsTemplate : TemplateObjectBase
    {
        [JsonProperty("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("actions")]
        public IEnumerable<TemplateActionBase> Actions { get; set; }
    }

    [JsonSubtype("confirm")]
    public class ConfirmTemplate : TemplateObjectBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("actions")]
        public IEnumerable<TemplateActionBase> Actions { get; set; }
    }

    [JsonSubtype("carousel")]
    public class CarouselTemplate : TemplateObjectBase
    {
        [JsonProperty("columns")]
        public IEnumerable<ColumnObject> Columns { get; set; }
    }

    public class ColumnObject
    {
        [JsonProperty("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("actions")]
        public IEnumerable<TemplateActionBase> Actions { get; set; }
    }

    /// <summary>
    /// https://developers.line.me/en/docs/messaging-api/reference/#action-objects
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes))]
    public class TemplateActionBase
    {
        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    [JsonSubtype("postback")]
    public class PostbackTemplateAction : TemplateActionBase
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    [JsonSubtype("message")]
    public class MessageTemplateAction : TemplateActionBase
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    [JsonSubtype("uri")]
    public class UriTemplateAction : TemplateActionBase
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("uri")]
        public string Url { get; set; }
    }

    [JsonSubtype("datetimepicker")]
    public class DateTimePickerTemplateAction : TemplateActionBase
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("mode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DateTimePickerMode Mode { get; set; }

        [JsonProperty("initial")]
        public string Initial { get; set; }

        [JsonProperty("max")]
        public string Max { get; set; }

        [JsonProperty("min")]
        public string Min { get; set; }
    }

    public enum DateTimePickerMode
    {
        [EnumMember(Value = "date")]
        Date,

        [EnumMember(Value = "time")]
        Time,

        [EnumMember(Value = "datetime")]
        DateTime,
    }
}