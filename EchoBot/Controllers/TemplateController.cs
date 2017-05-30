using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using LineSharp;
using LineSharp.Messages;

namespace EchoBot.Controllers
{
    [RoutePrefix("api/template")]
    public class TemplateController : ApiController
    {
        private LineClient Client;

        public TemplateController()
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
                                    if (Regex.IsMatch(message.Text, "buttons", RegexOptions.IgnoreCase))
                                    {
                                        await SendButtonsAsync(mev.ReplyToken);
                                    }
                                    if (Regex.IsMatch(message.Text, "confirm", RegexOptions.IgnoreCase))
                                    {
                                        await SendConfirmAsync(mev.ReplyToken);
                                    }
                                    if (Regex.IsMatch(message.Text, "carousel", RegexOptions.IgnoreCase))
                                    {
                                        await SendCarouselAsync(mev.ReplyToken);
                                    }

                                    break;
                                }
                        }
                        break;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        private async Task SendButtonsAsync(string replyToken)
        {
            await Client.ReplyMessageAsync(replyToken, new TemplateMessage
            {
                AltText = "buttons",
                Template = new ButtonsTemplate
                {
                    ThumbnailImageUrl = Url.Content("~/Images/abstract-q-c-1024-678-3.jpg"),
                    Text = "Buttons Sample",
                    Title = "Title",
                    Actions = new TemplateActionBase[]
                    {
                        new MessageTemplateAction
                        {
                            Label = "Message",
                            Text = "Message Text",
                        },
                        new UriTemplateAction
                        {
                            Label = "Uri",
                            Url = "https://github.com/masaedw/linesharp"
                        },
                        new PostbackTemplateAction
                        {
                            Label = "Postback with text",
                            Text = "postback text",
                            Data = "hoge=fuga&foo=bar"
                        },
                    },
                },
            });
        }

        private async Task SendConfirmAsync(string replyToken)
        {
            await Client.ReplyMessageAsync(replyToken, new TemplateMessage
            {
                AltText = "confirm",
                Template = new ConfirmTemplate
                {
                    Text = "Confirm Sample",
                    Actions = new[]
                    {
                        new MessageTemplateAction
                        {
                            Label = "OK Label",
                            Text = "OK Text",
                        },
                        new MessageTemplateAction
                        {
                            Label = "NG Label",
                            Text = "NG Text"
                        },
                    },
                },
            });
        }

        private async Task SendCarouselAsync(string replyToken)
        {
            await Client.ReplyMessageAsync(replyToken, new TemplateMessage
            {
                AltText = "carousel",
                Template = new CarouselTemplate
                {
                    Columns = new[]
                    {
                        new ColumnObject
                        {
                            ThumbnailImageUrl = Url.Content("~/Images/abstract-q-c-1024-678-3.jpg"),
                            Title = "Abstract",
                            Text = "This is an abstract image.",
                            Actions = new [] {
                                new MessageTemplateAction
                                {
                                    Label = "abstract label",
                                    Text = "abstract",
                                },
                            },
                        },
                        new ColumnObject
                        {
                            ThumbnailImageUrl = Url.Content("~/Images/animals-q-c-1024-678-1.jpg"),
                            Title = "Animals",
                            Text = "This is an animals image.",
                            Actions = new [] {
                                new MessageTemplateAction
                                {
                                    Label = "animals label",
                                    Text = "animals",
                                },
                            },
                        },
                        new ColumnObject
                        {
                            ThumbnailImageUrl = Url.Content("~/Images/nature-q-c-1024-678-7.jpg"),
                            Title = "Nature",
                            Text = "This is an nature image.",
                            Actions = new [] {
                                new MessageTemplateAction
                                {
                                    Label = "nature label",
                                    Text = "nature",
                                },
                            },
                        },
                    },
                },
            });
        }
    }
}
