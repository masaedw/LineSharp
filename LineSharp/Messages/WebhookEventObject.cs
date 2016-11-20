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
        public MessageObject Message { get; set; }
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

    public class MessageObject
    {
        public string id { get; set; }
        public string type { get; set; }

        // text
        public string text { get; set; }

        // location
        public string title { get; set; }

        // location
        public string address { get; set; }

        // location
        public decimal latitude { get; set; }

        // location
        public decimal longitude { get; set; }

        // sticker
        public string packageId { get; set; }

        // sticker
        public string stickerId { get; set; }
    }

    public class PostbackObject
    {
        public string data { get; set; }
    }

    public class BeaconObject
    {
        public string hwid { get; set; }
        public string type { get; set; }
    }
}
