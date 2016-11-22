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

    public static class EventType
    {
        public const string Message = "message";
        public const string Follow = "follow";
        public const string Unfollow = "unfollow";
        public const string Join = "join";
        public const string Leave = "leave";
        public const string Postback = "postback";
        public const string Beacon = "beacon";
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

    [JsonSubtype(EventType.Message)]
    public class MessageEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("message")]
        public MessageObjectBase Message { get; set; }
    }

    [JsonSubtype(EventType.Follow)]
    public class FollowEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
    }

    [JsonSubtype(EventType.Unfollow)]
    public class UnfollowEvent : WebhookEventBase
    {
    }

    [JsonSubtype(EventType.Join)]
    public class JoinEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
    }

    [JsonSubtype(EventType.Leave)]
    public class LeaveEvent : WebhookEventBase
    {
    }

    [JsonSubtype(EventType.Postback)]
    public class PostbackEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("postback")]
        public PostbackObject Postback { get; set; }
    }

    [JsonSubtype(EventType.Beacon)]
    public class BeaconEvent : WebhookEventBase, IReplyableEvent
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("beacon")]
        public BeaconObject Beacon { get; set; }
    }

    public static class SourceType
    {
        public const string User = "user";
        public const string Group = "group";
        public const string Room = "room";
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

    [JsonSubtype(SourceType.User)]
    public class UserSoruce : SourceObjectBase
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        public override string Id => UserId;
    }

    [JsonSubtype(SourceType.Group)]
    public class GroupSoruce : SourceObjectBase
    {
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        public override string Id => GroupId;
    }

    [JsonSubtype(SourceType.Room)]
    public class RoomSoruce : SourceObjectBase
    {
        [JsonProperty("roomId")]
        public string RoomId { get; set; }

        public override string Id => RoomId;
    }

    public static class MessageType
    {
        public const string Text = "text";
        public const string Image = "image";
        public const string Video = "video";
        public const string Audio = "audio";
        public const string Location = "location";
        public const string Sticker = "sticker";
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

    [JsonSubtype(MessageType.Text)]
    public class TextMessage : MessageObjectBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    [JsonSubtype(MessageType.Image)]
    public class ImageMessage : MessageObjectBase
    {
    }

    [JsonSubtype(MessageType.Video)]
    public class VideoMessage : MessageObjectBase
    {
    }

    [JsonSubtype(MessageType.Audio)]
    public class AudioMessage : MessageObjectBase
    {
    }

    [JsonSubtype(MessageType.Location)]
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

    [JsonSubtype(MessageType.Sticker)]
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
