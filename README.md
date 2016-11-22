# LineSharp

A binding of Line Messaging API in .NET Framework.

# What is Line Messaging API

See https://developers.line.me/messaging-api/overview

# Echo bot

In ASP.NET Web API2. 

```csharp
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LineSharp;
using LineSharp.Messages;

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
                    case "message":
                        var mev = (MessageEvent)ev;
                        switch (mev.Message.Type)
                        {
                            case "text":
                                var message = (TextMessage)mev.Message;
                                await Client.ReplyTextAsync(mev.ReplyToken, message.Text);
                                break;
                        }
                        break;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
    }
}
```

# Install

```powershell
Install-Package LineSharp -Pre
```

