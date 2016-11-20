using Newtonsoft.Json;

namespace LIneSharp.Messages
{
    public class ErrorResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public ErrorDetail[] Details { get; set; }
    }

    public class ErrorDetail
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("property")]
        public string Property { get; set; }
    }
}
