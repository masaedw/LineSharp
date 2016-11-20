using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LineSharp.Json;
using LIneSharp.Messages;
using Newtonsoft.Json;

namespace LineSharp
{
    public class LineClient
    {
        static LineClient()
        {
            JsonSubtypes.autoRegister(Assembly.GetExecutingAssembly());
        }

        public static readonly string DefaultLineUrlPrefix = "https://api.line.me/v2/bot/";

        public string ChannelId { get; }
        public string ChannelSecret { get; }
        public string ChannelAccessToken { get; }

        public string ApiUrlPrefix { get; set; } = DefaultLineUrlPrefix;
        public JsonMediaTypeFormatter Formatter { get; }

        public List<DelegatingHandler> Handlers { get; } = new List<DelegatingHandler>();

        public LineClient(string channelId, string channelSecret, string accessToken)
        {
            ChannelId = channelId;
            ChannelSecret = channelSecret;
            ChannelAccessToken = accessToken;
            Formatter = new JsonMediaTypeFormatter();
            Formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public IEnumerable<WebhookEventBase> ParseEvent(string content)
        {
            return JsonConvert.DeserializeObject<WebhookEventRequest>(content).events;
        }

        public async Task HandleEventAsync(WebhookEventBase ev)
        {
            if (ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            switch (ev.Type)
            {
                case "message":
                    await HandleMessage((MessageEvent)ev);
                    return;

                case "follow":
                    await HandleFollow((FollowEvent)ev);
                    return;

                case "unfollow":
                    await HandleUnfollow((UnfollowEvent)ev);
                    return;

                case "join":
                    await HandleJoin((JoinEvent)ev);
                    return;

                case "leave":
                    await HandleLeave((LeaveEvent)ev);
                    return;

                case "postback":
                    await HandlePostback((PostbackEvent)ev);
                    return;

                case "beacon":
                    await HandleBeacon((BeaconEvent)ev);
                    return;

                default:
                    throw new LineException($"unknown event type: {ev.Type}");
            }
        }

        public virtual Task HandleMessage(MessageEvent ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleFollow(FollowEvent ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleUnfollow(UnfollowEvent ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleJoin(JoinEvent ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleLeave(LeaveEvent ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandlePostback(PostbackEvent ev)
        {
            return Task.CompletedTask;
        }

        public virtual Task HandleBeacon(BeaconEvent ev)
        {
            return Task.CompletedTask;
        }

        public bool ValidateSignature(string content, string channelSignature)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (channelSignature == null)
            {
                throw new ArgumentNullException(nameof(channelSignature));
            }

            var hasher = new HMACSHA256(Convert.FromBase64String(ChannelAccessToken));
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(content));

            return SlowEquals(hash, Convert.FromBase64String(channelSignature));
        }

        // http://stackoverflow.com/questions/16953231/cryptography-net-avoiding-timing-attack
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        public async Task PostAsync<TMessage>(string url, TMessage msg)
        {
            using (var client = HttpClientFactory.Create(Handlers.ToArray()))
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
            using (var client = HttpClientFactory.Create(Handlers.ToArray()))
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
