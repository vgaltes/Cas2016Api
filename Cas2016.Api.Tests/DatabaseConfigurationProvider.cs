using System;
using System.IO;

namespace Cas2016.Api.Tests
{
    internal class DatabaseConfigurationProvider
    {
        private const string DatabaseProjectName = "Cas2016Api.Db";
        private static DatabaseConfigurationProvider _instance;

        public static DatabaseConfigurationProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseConfigurationProvider();
                }

                return _instance;
            }
        }

        public string DacPacFilePath { get; }

        public string TargetConnectionString { get; }

        public string DatabaseTargetName { get; }

        private DatabaseConfigurationProvider()
        {
            DatabaseTargetName = "Cas2016Api.Db";

            TargetConnectionString =
                $"Server=(localdb)\\MSSQLLocalDB;Integrated Security=true; Initial Catalog = {DatabaseTargetName}";

            DacPacFilePath = AppDomain.CurrentDomain.BaseDirectory + $"\\Db\\{DatabaseProjectName}.dacpac";
        }
    }
}