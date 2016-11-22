using System;
using System.Collections.Generic;
using LineSharp.Json;
using Newtonsoft.Json;

namespace LineSharp.Messages
{
    public class WebhookEventRequest
    {
        [JsonProperty("events")]
        public IEnumerable<WebhookEventBase> Events { get; set; }
    }

    public interface IReplyableEvent
    {
        string ReplyToken { get; }
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
        public SourceObjectBase Source { get; set; }
    }

    [JsonSubtype("message")]
    public class MessageEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("message")]
        public MessageObjectBase Message { get; set; }
    }

    [JsonSubtype("follow")]
    public class FollowEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
    }

    [JsonSubtype("unfollow")]
    public class UnfollowEvent : WebhookEventBase
    {
    }

    [JsonSubtype("join")]
    public class JoinEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
    }

    [JsonSubtype("leave")]
    public class LeaveEvent : WebhookEventBase
    {
    }

    [JsonSubtype("postback")]
    public class PostbackEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("postback")]
        public PostbackObject Postback { get; set; }
    }

    [JsonSubtype("beacon")]
    public class BeaconEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("beacon")]
        public BeaconObject Beacon { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes))]
    public class SourceObjectBase
    {
        [JsonTag]
        [JsonProperty("type")]
        public string Type { get; set; }

        public virtual string Id
        {
            get
            {
                throw new NotImplementedException(); // dummy
            }
        }
    }

    [JsonSubtype("user")]
    public class UserSoruce : SourceObjectBase
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        public override string Id => UserId;
    }

    [JsonSubtype("group")]
    public class GroupSoruce : SourceObjectBase
    {
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        public override string Id => GroupId;
    }

    [JsonSubtype("room")]
    public class RoomSoruce : SourceObjectBase
    {
        [JsonProperty("roomId")]
        public string RoomId { get; set; }

        public override string Id => RoomId;
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
