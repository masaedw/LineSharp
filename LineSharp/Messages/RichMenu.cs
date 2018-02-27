using Newtonsoft.Json;

namespace LineSharp.Messages
{
    public class RichMenuId
    {
        [JsonProperty("richMenuId")]
        public string Id { get; set; }
    }
}