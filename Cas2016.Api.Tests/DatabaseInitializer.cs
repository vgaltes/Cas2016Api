using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Microsoft.SqlServer.Dac;

namespace Cas2016.Api.Tests
{
    /// <summary>
    ///     Class to initialise the database independant (as far as possible) of the database access method used to query data.
    /// </summary>
    public class DatabaseInitialiser
    {
        private readonly string _dacpacFilePath;
        private readonly string _databaseTargetName;
        private readonly string _targetConnectionString;

        public DatabaseInitialiser()
        {
            _databaseTargetName = DatabaseConfigurationProvider.Instance.DatabaseTargetName;
            _targetConnectionString = DatabaseConfigurationProvider.Instance.TargetConnectionString;
            _dacpacFilePath = DatabaseConfigurationProvider.Instance.DacPacFilePath;
        }

        public void Publish(bool dropDatabase)
        {
            var dacServices = new DacServices(_targetConnectionString);

            //Wire up events for Deploy messages and for task progress (For less verbose output, don't subscribe to Message Event (handy for debugging perhaps?)
            dacServices.Message += DacServicesHandlers.Message;
            dacServices.ProgressChanged += DacServicesHandlers.ProgressChanged;

            var dbPackage = DacPackage.Load(_dacpacFilePath);
            var dbDeployOptions = new DacDeployOptions {CreateNewDatabase = dropDatabase};

            dacServices.Deploy(dbPackage, _databaseTargetName, true, dbDeployOptions);
        }

        public void Seed(string[] scriptFilePaths)
        {
            using (var connection = new SqlConnection(_targetConnectionString))
            {
                connection.Open();

                foreach (var seedScript in scriptFilePaths)
                    ExecuteScript(seedScript, connection);

                connection.Close();
            }
        }

        private static void ExecuteScript(string seedScript, SqlConnection connection)
        {
            var commandText = File.ReadAllText(seedScript);

            using (var command = new SqlCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static class DacServicesHandlers
        {
            public static void Message(object sender, DacMessageEventArgs e)
            {
                Debug.WriteLine("DAC Message: {0}", e.Message);
            }

            public static void ProgressChanged(object sender, DacProgressEventArgs e)
            {
                Debug.WriteLine(e.Status + ": " + e.Message);
            }
        }
    }
}