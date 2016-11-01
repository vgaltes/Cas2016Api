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
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

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

            builder
                .RegisterType<RoomRepository>()
                .As<IRoomRepository>()
                .WithParameter("connectionString", ConfigurationProvider.DbConnectionString);

            var container = builder.Build();
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacWebApi(configuration);

        }
    }
}