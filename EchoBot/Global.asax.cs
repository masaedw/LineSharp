using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using LineSharp;

namespace EchoBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LineClient.Initialize();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
