using System.Collections.Generic;
using Newtonsoft.Json;

namespace LineSharp.Messages
{
    internal class RichMenuId
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

    internal class RichMenuList
    {
        [JsonProperty("richmenus")]
        public IEnumerable<RichMenuResponse> RichMenus { get; set; }
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
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }
    }
}