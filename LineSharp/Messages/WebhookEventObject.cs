namespace LIneSharp.Messages
{
    /// <summary>
    /// https://devdocs.line.me/#webhook-event-object
    /// </summary>
    public class WebhookEventObject
    {
        // common fields
        public string type { get; set; }

        public ulong timestamp { get; set; }

        public SourceObject source { get; set; }

        // message, follow, join
        public string replyToken { get; set; }

        // message
        public MessageObject message { get; set; }

        // postback
        public PostbackObject postback { get; set; }

        // beacon
        public BeaconObject beacon { get; set; }
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

        // TODO: Not implemented yet: Image, Video, Audio, Location, Sticker
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
