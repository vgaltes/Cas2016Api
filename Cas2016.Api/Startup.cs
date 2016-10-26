﻿using System.Web.Http;
using Owin;

namespace Cas2016.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            configuration.Routes.MapHttpRoute("Default", "", new {controller = "default"});
            configuration.Routes.MapHttpRoute("Sessions", "sessions", new {controller = "sessions"});

            app.UseWebApi(configuration);
        }
    }
}