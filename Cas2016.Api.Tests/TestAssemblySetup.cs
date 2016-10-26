using System;
using NUnit.Framework;

namespace Cas2016.Api.Tests
{
    [SetUpFixture]
    public class TestAssemblySetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTestsInThisAssembly()
        {
            var dbInitialiser = new DatabaseInitialiser();

            // ReSharper disable once ConvertToConstant.Local
            var shouldDropAndCreateDatabase = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!shouldDropAndCreateDatabase)
                return;

            dbInitialiser.Publish(true);

            var scriptsBasePath = AppDomain.CurrentDomain.BaseDirectory + @"\Scripts\";

            var scriptFilePaths = new[]
            {
                scriptsBasePath + "InsertSessions.sql"
            };

            dbInitialiser.Seed(scriptFilePaths);
        }
    }
}