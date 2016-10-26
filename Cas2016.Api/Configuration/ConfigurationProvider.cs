using System.Configuration;

namespace Cas2016.Api.Configuration
{
    public static class ConfigurationProvider
    {
        private const string ConnectionStringKey = "Cas2016Api.Db";

        public static string DbConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
                return connectionString;
            }
        }
    }
}