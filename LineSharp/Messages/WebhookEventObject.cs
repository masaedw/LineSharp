using System.Collections.Generic;
using LineSharp.Json;
using Newtonsoft.Json;

namespace LIneSharp.Messages
{
    public class WebhookEventRequest
    {
        public IEnumerable<WebhookEventBase> events { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes))]
    public class WebhookEventBase
    {
        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }

        [JsonProperty("source")]
        public SourceObject Source { get; set; }
    }

    [JsonSubtype("message")]
    public class MessageEvent : WebhookEventBase
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("message")]
        public MessageObjectBase Message { get; set; }
    }

    [JsonSubtype("follow")]
    public class FollowEvent : WebhookEventBase
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
    }

    [JsonSubtype("unfollow")]
    public class UnfollowEvent : WebhookEventBase
    {
    }

    [JsonSubtype("join")]
    public class JoinEvent : WebhookEventBase
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
    }

    [JsonSubtype("leave")]
    public class LeaveEvent : WebhookEventBase
    {
    }

    [JsonSubtype("postback")]
    public class PostbackEvent : WebhookEventBase
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("postback")]
        public PostbackObject Postback { get; set; }
    }

    [JsonSubtype("beacon")]
    public class BeaconEvent : WebhookEventBase
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("beacon")]
        public BeaconObject Beacon { get; set; }
    }

    public class SourceObject
    {
        public string type { get; set; }

        // user
        public string userId { get; set; }

        // group
        public string groupId { get; set; }

        // room
        public string roomId { get; set; }

        public string GetId()
        {
            switch (type)
            {
                case "user":
                    return userId;

                case "group":
                    return groupId;

                case "room":
                    return roomId;
            }

            return null;
        }
    }

    [JsonConverter(typeof(JsonSubtypes))]
    public class MessageObjectBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    [JsonSubtype("text")]
    public class TextMessage : MessageObjectBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    [JsonSubtype("image")]
    public class ImageMessage : MessageObjectBase
    {
    }

    [JsonSubtype("video")]
    public class VideoMessage : MessageObjectBase
    {
    }

    [JsonSubtype("audio")]
    public class AudioMessage : MessageObjectBase
    {
    }

    [JsonSubtype("location")]
    public class LocationMessage : MessageObjectBase
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

    [JsonSubtype("sticker")]
    public class StickerMessage : MessageObjectBase
    {
        [JsonProperty("packageId")]
        public string PackageId { get; set; }

        [JsonProperty("stickerId")]
        public string StickerId { get; set; }
    }

    public class PostbackObject
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }

    public class BeaconObject
    {
        [JsonProperty("hwid")]
        public string Hwid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
