using System.Collections.Generic;

namespace LIneSharp.Messages
{
    /// <summary>
    /// https://devdocs.line.me/#reply-message
    /// </summary>
    public class ReplyMessage
    {
        public string replyToken { get; set; }
        public IEnumerable<SendMessageObject> messages { get; set; }
    }

    /// <summary>
    /// https://devdocs.line.me/#push-message
    /// </summary>
    public class PushMessage
    {
        public string to { get; set; }
        public IEnumerable<SendMessageObject> messages { get; set; }
    }

    /// <summary>
    /// https://devdocs.line.me/#send-message-object
    /// </summary>
    public class SendMessageObject
    {
        // common
        public string type { get; set; }

        // text
        public string text { get; set; }

        // image, video, audio
        public string originalContentUrl { get; set; }

        // image, video
        public string previewImageUrl { get; set; }

        // audio
        public double duration { get; set; }

        // location
        public string title { get; set; }

        public string address { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        // sticker
        public string packageId { get; set; }

        public string stickerId { get; set; }
    }
}
