using System.Configuration;

namespace Cas2016.Api.Configuration
{
    public class ConfigurationProvider
    {
        private const string ConnectionStringKey = "Cas2016Api.Db";

        public string DbConnectionString => ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
    }
}