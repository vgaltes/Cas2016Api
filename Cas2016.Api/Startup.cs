using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Cas2016.Api.Configuration;
using Cas2016.Api.Repositories;
using Owin;

namespace Cas2016.Api
{
    public class Startup
    {
        public static void AddRoutes(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("Default", "", new { controller = "default" });
            //configuration.Routes.MapHttpRoute("Sessions", "sessions/{sessionId}", new { controller = "sessions", sessionId = RouteParameter.Optional });
            configuration.Routes.MapHttpRoute("Speakers", "speakers/{speakerId}", new {controller = "speakers", speakerId = RouteParameter.Optional });
        }

        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            AddRoutes(configuration);
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);


            ConfigureAutoFac(app, configuration);

            configuration.MapHttpAttributeRoutes();
            app.UseWebApi(configuration);
        }

        private void ConfigureAutoFac(IAppBuilder app, HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            // Register Web API controller in executing assembly.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder
                .RegisterType<SpeakerRepository>()
                .As<ISpeakerRepository>()
                .WithParameter("connectionString", ConfigurationProvider.DbConnectionString);

            builder
                .RegisterType<SessionRepository>()
                .As<ISessionRepository>()
                .WithParameter("connectionString", ConfigurationProvider.DbConnectionString);



            var container = builder.Build();
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacWebApi(configuration);

        }
    }
}