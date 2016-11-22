using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using LineSharp;
using LineSharp.Messages;
using NReco.VideoConverter;

namespace EchoBot.Controllers
{
    [RoutePrefix("api/line")]
    public class LineController : ApiController
    {
        private LineClient Client;

        public LineController()
        {
            var channelId = Environment.GetEnvironmentVariable("LINE_CHANNEL_ID");
            var channelSecret = Environment.GetEnvironmentVariable("LINE_CHANNEL_SECRET");
            var accessToken = Environment.GetEnvironmentVariable("LINE_CHANNEL_ACCESS_TOKEN");
            Client = new LineClient(channelId, channelSecret, accessToken);
        }

        [Route("webhook")]
        public async Task<HttpResponseMessage> Post()
        {
            var content = await Request.Content.ReadAsStringAsync();
            var signature = Request.Headers.GetValues("X-Line-Signature").FirstOrDefault();
            if (signature == null || !Client.ValidateSignature(content, signature))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "not found");
            }

            var events = Client.ParseEvent(content);

            foreach (var ev in events)
            {
                switch (ev.Type)
                {
                    case EventType.Message:
                        var mev = (MessageEvent)ev;
                        switch (mev.Message.Type)
                        {
                            case MessageType.Text:
                                {
                                    var message = (TextEventMessage)mev.Message;
                                    await Client.ReplyTextAsync(mev.ReplyToken, message.Text);
                                    break;
                                }

                            case MessageType.Image:
                                {
                                    var message = (ImageEventMessage)mev.Message;
                                    var image = await Client.GetContentAsync(message.Id);
                                    var key = AddToCache(image);
                                    await Client.ReplyMessageAsync(mev.ReplyToken, new ImageMessage
                                    {
                                        OriginalContentUrl = GetContentUrl(key),
                                        PreviewImageUrl = GetContentUrl(key),
                                    });
                                    break;
                                }

                            case MessageType.Video:
                                {
                                    var message = (VideoEventMessage)mev.Message;
                                    var video = await Client.GetContentAsync(message.Id);
                                    var key = AddToCache(video);
                                    var thumb = MakeThumbnail(video);
                                    var thumbkey = AddToCache(thumb);
                                    await Client.ReplyMessageAsync(mev.ReplyToken, new VideoMessage
                                    {
                                        OriginalContentUrl = GetContentUrl(key),
                                        PreviewImageUrl = GetContentUrl(thumbkey),
                                    });
                                    break;
                                }

                            case MessageType.Audio:
                                {
                                    var message = (AudioEventMessage)mev.Message;
                                    var audio = await Client.GetContentAsync(message.Id);
                                    var key = AddToCache(audio);
                                    await Client.ReplyMessageAsync(mev.ReplyToken, new AudioMessage
                                    {
                                        OriginalContentUrl = GetContentUrl(key),
                                        Duration = 0.5,
                                    });
                                    break;
                                }

                            case MessageType.Location:
                                {
                                    var message = (LocationEventMessage)mev.Message;
                                    await Client.ReplyMessageAsync(mev.ReplyToken, new LocationMessage
                                    {
                                        Address = message.Address,
                                        Title = message.Title,
                                        Latitude = message.Latitude,
                                        Longitude = message.Longitude,
                                    });
                                    break;
                                }

                            case MessageType.Sticker:
                                {
                                    var message = (StickerEventMessage)mev.Message;
                                    await Client.ReplyMessageAsync(mev.ReplyToken, new StickerMessage
                                    {
                                        PackageId = message.PackageId,
                                        StickerId = message.StickerId,
                                    });
                                    break;
                                }
                        }
                        break;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [Route("content/{key}", Name = "GetContent")]
        public HttpResponseMessage GetContent(string key)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(GetFromCache(key));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        private string AddToCache(byte[] data)
        {
            var cache = MemoryCache.Default;
            var key = Guid.NewGuid().ToString();
            cache.Add(key, data, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) });
            return key;
        }

        private byte[] GetFromCache(string key)
        {
            var cache = MemoryCache.Default;
            return (byte[])cache.Get(key);
        }

        private string GetContentUrl(string key)
        {
            return Url.Link("GetContent", new { key });
        }

        private byte[] MakeThumbnail(byte[] video)
        {
            var temp = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(temp, video);
                var converter = new FFMpegConverter();
                var stream = new MemoryStream();
                converter.GetVideoThumbnail(temp, stream);
                return stream.ToArray();
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}
