using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LineSharp.Messages;
using Newtonsoft.Json;

namespace LineSharp.Rest
{
    public class RestClient : IRestClient
    {
        public List<DelegatingHandler> HttpHandlers { get; } = new List<DelegatingHandler>();
        public JsonMediaTypeFormatter Formatter { get; }
        public string UrlPrefix { get; }
        public string ChannelAccessToken { get; }

        public RestClient(string urlPrefix, string channelAccessToken)
        {
            Formatter = new JsonMediaTypeFormatter();
            Formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            UrlPrefix = urlPrefix;
            ChannelAccessToken = channelAccessToken;
        }

        public async Task<TResponse> PostAsync<TResponse>(string url, object msg)
        {
            using (var client = HttpClientFactory.Create(HttpHandlers.ToArray()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.PostAsync($"{UrlPrefix}{url}", msg, Formatter).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
                    throw new LineException(body.Message, body);
                }

                return JsonConvert.DeserializeObject<TResponse>(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
        }

        public async Task PostAsync(string url, object msg)
        {
            using (var client = HttpClientFactory.Create(HttpHandlers.ToArray()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.PostAsync($"{UrlPrefix}{url}", msg, Formatter).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
                    throw new LineException(body.Message, body);
                }
            }
        }

        public async Task PostByteArrayAsync(string url, byte[] msg)
        {
            using (var client = HttpClientFactory.Create(HttpHandlers.ToArray()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var content = new ByteArrayContent(msg);
                var res = await client.PostAsync($"{UrlPrefix}{url}", content).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
                    throw new LineException(body.Message, body);
                }
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            using (var client = HttpClientFactory.Create(HttpHandlers.ToArray()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.GetAsync($"{UrlPrefix}{url}").ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>();
                    throw new LineException(body.Message, body);
                }

                return JsonConvert.DeserializeObject<TResponse>(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
        }

        public async Task<byte[]> GetAsyncAsByteArray(string url)
        {
            using (var client = HttpClientFactory.Create(HttpHandlers.ToArray()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.GetAsync($"{UrlPrefix}{url}").ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>();
                    throw new LineException(body.Message, body);
                }

                return await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(string url)
        {
            using (var client = HttpClientFactory.Create(HttpHandlers.ToArray()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.DeleteAsync($"{UrlPrefix}{url}").ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>();
                    throw new LineException(body.Message, body);
                }
            }
        }
    }
}