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
                switch (ev)
                {
                    case MessageEvent mev:
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
                                    if (Regex.IsMatch(message.Text, "datetime", RegexOptions.IgnoreCase))
                                    {
                                        await SendDateTimePickerAsync(mev.ReplyToken);
                                    }

                                    break;
                                }
                        }
                        break;

                    case PostbackEvent pev:
                        var text = $"Received a PostbackEvent\nData: {pev.Postback.Data}\nParams.Date: {pev.Postback.Params?.Date}\nParams.Time: {pev.Postback.Params?.Time}\nParams.DateTime: {pev.Postback.Params?.DateTime}";
                        await Client.ReplyTextAsync(pev.ReplyToken, text);

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

        private async Task SendDateTimePickerAsync(string replyToken)
        {
            var now = DateTime.Now;
            await Client.ReplyMessageAsync(replyToken, new TemplateMessage
            {
                AltText = "datetime pickers",
                Template = new ButtonsTemplate
                {
                    ThumbnailImageUrl = Url.Content("~/Images/abstract-q-c-1024-678-3.jpg"),
                    Text = "Datetime Picker Sample",
                    Title = "Title",
                    Actions = new TemplateActionBase[]
                    {
                        new DateTimePickerTemplateAction
                        {
                            Label = "Date",
                            Data = "data of date",
                            Mode = DateTimePickerMode.Date,
                            Initial = now.ToString("yyyy-MM-dd"),
                            Min = now.AddDays(-30).ToString("yyyy-MM-dd"),
                            Max = now.AddDays(30).ToString("yyyy-MM-dd"),
                        },
                        new DateTimePickerTemplateAction
                        {
                            Label = "Time",
                            Data = "data of time",
                            Mode = DateTimePickerMode.Time,
                            Initial = "12:00",
                            Min = "09:00",
                            Max = "21:00",
                        },
                        new DateTimePickerTemplateAction
                        {
                            Label = "DateTime",
                            Data = "data of datetime",
                            Mode = DateTimePickerMode.DateTime,
                            Initial = now.ToString("yyyy-MM-ddT12:00"),
                            Min = now.AddDays(-30).ToString("yyyy-MM-ddT00:00"),
                            Max = now.AddDays(30).ToString("yyyy-MM-ddT23:59"),
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