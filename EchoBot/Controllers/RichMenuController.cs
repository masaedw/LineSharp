using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using LineSharp;
using LineSharp.Messages;

namespace EchoBot.Controllers
{
    [RoutePrefix("api/richrps")]
    public class RichMenuController : ApiController
    {
        private LineClient Client;

        public RichMenuController()
        {
            var channelId = Environment.GetEnvironmentVariable("LINE_CHANNEL_ID");
            var channelSecret = Environment.GetEnvironmentVariable("LINE_CHANNEL_SECRET");
            var accessToken = Environment.GetEnvironmentVariable("LINE_CHANNEL_ACCESS_TOKEN");

            channelId = "1489026731";
            channelSecret = "b03ae87de61f2d0c94c75adbb5bb33bb";
            accessToken = "UcaBvvitF/5YLrh97p3vAXVX4qROTtHAEc4aWhwukt3mWFXl0YzGCeUHJrKhdWXdmwgDG3mNMmb7aEPp3u1rIwEK04/lmM83PiZClWnYeM2TwSvhpNyPQ0T/Tn09PG/BBI7GEdpiN7/7LJb+UTqhUgdB04t89/1O/w1cDnyilFU=";

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
                        switch (mev.Message)
                        {
                            case TextEventMessage message when Regex.IsMatch(message.Text, "start", RegexOptions.IgnoreCase):
                                await SetupRichMenuAsync(mev.ReplyToken, mev.Source.Id);
                                break;

                            case TextEventMessage message when Regex.IsMatch(message.Text, "finish", RegexOptions.IgnoreCase):
                                await ResetRichMenuAsync(mev.ReplyToken, mev.Source.Id);
                                break;

                            case TextEventMessage message when Regex.IsMatch(message.Text, "list", RegexOptions.IgnoreCase):
                                await ListRichMenus(mev.ReplyToken);
                                break;
                        }
                        break;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        private async Task SetupRichMenuAsync(string replyToken, string userId)
        {
            // Create a rich menu and then link it to user

            // Actually it is not necessary to create a rich menu each time.
            // Since it can only create up to 10, it can not be used by multiple users.
            // But this is for testing.
            var menu = await Client.CreateRichMenu(new RichMenu
            {
                Size = new Size { Width = 2500, Height = 843 },
                Selected = true,
                Name = "solar system",
                ChatBarText = "solar system",
                Areas = new[]
                {
                    new Area
                    {
                        Bounds = new Bounds { X = 0, Y = 0, Width = 333, Height = 843 },
                        Action = new MessageTemplateAction { Text = "Sun", Label = "Sun" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 333, Y = 0, Width = 512-333, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Mercury", Label = "Mercury" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 512, Y = 0, Width = 616-512, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Venus", Label = "Venus" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 741, Y = 0, Width = 850-741, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Mars", Label = "Mars" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 850, Y = 0, Width = 1200-850, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Jupiter", Label = "Jupiter" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 1200, Y = 0, Width = 1548-1120, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Saturn", Label = "Saturn" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 1548, Y = 0, Width = 1713-1548, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Uranus", Label = "Uranus" },
                    },
                    new Area
                    {
                        Bounds = new Bounds { X = 1713, Y = 0, Width = 1962-1713, Height = 843/2 },
                        Action = new MessageTemplateAction { Text = "Neptune", Label = "Neptune" },
                    },
                },
            });

            var richMenuId = menu.RichMenuId;
            var bytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Images/solar_2500.jpg"));
            await Client.UploadRichMenuImage(richMenuId, "image/jpeg", bytes);
            await Client.LinkRichMenuToUser(userId, richMenuId);

            await Client.ReplyTextAsync(replyToken, $"Updated your rich menu.\nrichMenuId: {richMenuId}");
        }

        private async Task ResetRichMenuAsync(string replyToken, string userId)
        {
            var richMenuId = await Client.GetRichMenuOfUser(userId);
            await Client.UnlinkRichMenuFromUser(userId);

            // This is for testing.
            // Generally, in my opinion, you should reuse rich menus.
            await Client.DeleteRichMenu(richMenuId);

            await Client.ReplyTextAsync(replyToken, $"Deleted your rich menu.\nrichMenuId: {richMenuId}");
        }

        private async Task ListRichMenus(string replyToken)
        {
            var menus = await Client.GetRichMenuList();
            var ids = menus.Select(m => m.RichMenuId);

            var message = $"number of rich menus: {ids.Count()}\n{string.Join("\n", ids)}";

            await Client.ReplyTextAsync(replyToken, message);
        }
    }
}