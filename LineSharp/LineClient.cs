using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LIneSharp.Messages;
using Newtonsoft.Json;

namespace LineSharp
{
    public class LineClient
    {
        public string ChannelId { get; }
        public string ChannelSecret { get; }
        public string ChannelAccessToken { get; }

        public string ApiUrlPrefix { get; set; } = "https://api.line.me/v2/bot/";
        public JsonMediaTypeFormatter Formatter { get; }

        public LineClient(string id, string secret, string accessToken)
        {
            ChannelId = id;
            ChannelSecret = secret;
            ChannelAccessToken = accessToken;
            Formatter = new JsonMediaTypeFormatter();
            Formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public async Task HandleEventAsync(WebhookEventObject ev)
        {
            if (ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            switch (ev.type)
            {
                case "message":
                    await HandleMessage(ev);
                    return;

                case "follow":
                    await HandleFollow(ev);
                    return;

                case "unfollow":
                    await HandleUnfollow(ev);
                    return;

                case "join":
                    await HandleJoin(ev);
                    return;

                case "leave":
                    await HandleLeave(ev);
                    return;

                case "postback":
                    await HandlePostback(ev);
                    return;

                case "beacon":
                    await HandleBeacon(ev);
                    return;

                default:
                    throw new LineException($"unknown event type: {ev.type}");
            }
        }

        public virtual Task HandleMessage(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleFollow(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleUnfollow(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleJoin(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleLeave(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandlePostback(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleBeacon(WebhookEventObject ev)
        {
            return Task.CompletedTask;
        }

        public async Task PostAsync<TMessage>(string url, TMessage msg)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.PostAsync($"{ApiUrlPrefix}{url}", msg, Formatter);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>();
                    throw new LineException(body.message, body);
                }
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken);
                var res = await client.GetAsync($"{ApiUrlPrefix}{url}");
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsAsync<ErrorResponse>();
                    throw new LineException(body.message, body);
                }

                return JsonConvert.DeserializeObject<TResponse>(await res.Content.ReadAsStringAsync());
            }
        }

        public async Task PushTextAsync(string to, string text)
        {
            var msg = new PushMessage
            {
                to = to,
                messages = new[]
                {
                    new SendMessageObject { type = "text", text = text },
                },
            };
            await PostAsync("message/push", msg);
        }

        public async Task ReplyTextAsync(string replyToken, string text)
        {
            var msg = new ReplyMessage
            {
                replyToken = replyToken,
                messages = new[]
                {
                    new  SendMessageObject { type = "text", text = text },
                },
            };
            await PostAsync("message/reply", msg);
        }

        public async Task<UserObject> GetProfileAsync(string userId)
        {
            return await GetAsync<UserObject>($"profile/{userId}");
        }
    }
}
