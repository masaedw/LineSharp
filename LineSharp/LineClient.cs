﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LineSharp.Json;
using LineSharp.Messages;
using LineSharp.Rest;
using Newtonsoft.Json;

namespace LineSharp
{
    public class LineClient
    {
        private static bool Initialized = false;

        public static void Initialize()
        {
            if (!Initialized)
            {
                JsonSubtypes.autoRegister(Assembly.GetExecutingAssembly());
                Initialized = true;
            }
        }

        static LineClient()
        {
            Initialize();
        }

        public static readonly string DefaultLineUrlPrefix = "https://api.line.me/v2/bot/";
        public static Func<string, string, IRestClient> CreateRestCrient = (urlPrefix, channelAccessToken) => new RestClient(urlPrefix, channelAccessToken);

        public string ChannelId { get; }
        public string ChannelSecret { get; }
        private IRestClient RestClient { get; }

        public LineClient(string channelId, string channelSecret, string accessToken)
        {
            ChannelId = channelId;
            ChannelSecret = channelSecret;
            RestClient = CreateRestCrient(DefaultLineUrlPrefix, accessToken);
        }

        public IEnumerable<WebhookEventBase> ParseEvent(string content)
        {
            return JsonConvert.DeserializeObject<WebhookEventRequest>(content).Events;
        }

        public async Task HandleEventAsync(WebhookEventBase ev)
        {
            if (ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            switch (ev.Type)
            {
                case EventType.Message:
                    await HandleMessage((MessageEvent)ev)
                        .ConfigureAwait(false);
                    return;

                case EventType.Follow:
                    await HandleFollow((FollowEvent)ev)
                        .ConfigureAwait(false);
                    return;

                case EventType.Unfollow:
                    await HandleUnfollow((UnfollowEvent)ev)
                        .ConfigureAwait(false);
                    return;

                case EventType.Join:
                    await HandleJoin((JoinEvent)ev)
                        .ConfigureAwait(false);
                    return;

                case EventType.Leave:
                    await HandleLeave((LeaveEvent)ev)
                        .ConfigureAwait(false);
                    return;

                case EventType.Postback:
                    await HandlePostback((PostbackEvent)ev)
                        .ConfigureAwait(false);
                    return;

                case EventType.Beacon:
                    await HandleBeacon((BeaconEvent)ev)
                        .ConfigureAwait(false);
                    return;

                default:
                    throw new LineException($"unknown event type: {ev.Type}");
            }
        }

        protected virtual Task HandleMessage(MessageEvent ev)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleFollow(FollowEvent ev)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleUnfollow(UnfollowEvent ev)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleJoin(JoinEvent ev)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleLeave(LeaveEvent ev)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandlePostback(PostbackEvent ev)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleBeacon(BeaconEvent ev)
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

            var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(ChannelSecret));
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

        public Task PushMessageAsync(string to, SendMessageBase msg)
        {
            var p = new PushMessage
            {
                To = to,
                Messages = new[] { msg },
            };

            return RestClient.PostAsync("message/push", p);
        }

        public Task PushTextAsync(string to, string text)
        {
            return PushMessageAsync(to, new TextMessage { Text = text });
        }

        public Task ReplyMessageAsync(string replyToken, SendMessageBase msg)
        {
            var r = new ReplyMessage
            {
                ReplyToken = replyToken,
                Messages = new[] { msg },
            };

            return RestClient.PostAsync("message/reply", r);
        }

        public Task ReplyTextAsync(string replyToken, string text)
        {
            return ReplyMessageAsync(replyToken, new TextMessage { Text = text });
        }

        public Task<UserObject> GetProfileAsync(string userId)
        {
            return RestClient.GetAsync<UserObject>($"profile/{userId}");
        }

        public Task<byte[]> GetContentAsync(string messageId)
        {
            return RestClient.GetAsyncAsByteArray($"message/{messageId}/content");
        }
    }
}
