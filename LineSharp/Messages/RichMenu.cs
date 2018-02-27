using System.Collections.Generic;
using Newtonsoft.Json;

namespace LineSharp.Messages
{
    public class RichMenuId
    {
        [JsonProperty("richMenuId")]
        public string Id { get; set; }
    }

    /// <summary>
    /// https://developers.line.me/en/docs/messaging-api/reference/#rich-menu-object
    /// </summary>
    public class RichMenu
    {
        [JsonProperty("size")]
        public Size Size { get; set; }

        [JsonProperty("selected")]
        public bool Selected { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("chatBarText")]
        public string ChatBarText { get; set; }

        [JsonProperty("areas")]
        public IEnumerable<Area> Areas { get; set; }
    }

    /// <summary>
    /// https://developers.line.me/en/docs/messaging-api/reference/#rich-menu-response-object
    /// </summary>
    public class RichMenuResponse : RichMenu
    {
        [JsonProperty("richMenuId")]
        public string RichMenuId { get; set; }
    }

    /// <summary>
    /// https://developers.line.me/en/docs/messaging-api/reference/#area-object
    /// </summary>
    public class Area
    {
        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }

        [JsonProperty("action")]
        public TemplateActionBase Action { get; set; }
    }

    /// <summary>
    /// https://developers.line.me/en/docs/messaging-api/reference/#bounds-object
    /// </summary>
    public class Bounds
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
}