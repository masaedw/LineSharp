using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using LineSharp;
using LineSharp.Messages;

namespace EchoBot.Controllers
{
    [RoutePrefix("api/rps")]
    public class RPSController : ApiController
    {
        private LineClient Client;

        public RPSController()
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
                                    if (Regex.IsMatch(message.Text, "imagemap"))
                                    {
                                        await SendImageMapAsync(mev.ReplyToken);
                                    }

                                    break;
                                }
                        }
                        break;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        private async Task SendImageMapAsync(string replyToken)
        {
            await Client.ReplyMessageAsync(replyToken, new ImagemapMessage
            {
                BaseUrl = Url.Content("~/api/rps/image"),
                AltText = "rock paper scissors",
                Actions = new[]
                {
                    new MessageAction { Area = new ImagemapAreaObject { X = 346*0 + 0, Y = 0, Width = 346, Height = 650 }, Text = "Rock" },
                    new MessageAction { Area = new ImagemapAreaObject { X = 346*1 + 1, Y = 0, Width = 346, Height = 650 }, Text = "Paper" },
                    new MessageAction { Area = new ImagemapAreaObject { X = 346*2 + 2, Y = 0, Width = 346, Height = 650 }, Text = "Scissors" },
                },
                BaseSize = new Size { Height = 650, Width = 1040 },
            });
        }

        private Task SendUrlAsync(string replyToken)
        {
            throw new NotImplementedException();
        }

        [Route("image/{size}", Name = "GetImage")]
        public HttpResponseMessage GetImage(int size)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"EchoBot.Images.rps{size}.jpg";

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(assembly.GetManifestResourceStream(resourceName));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return response;
        }
    }
}
