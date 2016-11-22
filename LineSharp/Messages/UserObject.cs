using Newtonsoft.Json;

namespace LineSharp.Messages
{
    public class UserObject
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("pictureUrl")]
        public string PictureUrl { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }
    }
}
