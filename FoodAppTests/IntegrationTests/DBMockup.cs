using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using DbUp;

namespace FoodAppTests.IntegrationTests
{
    public class DBMockup : IAsyncLifetime
    {
        public MsSqlContainer _container;

        public string ConnectionString;

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();
            await _container.StartAsync();
            string connectionString = _container.GetConnectionString();
            ConnectionString = SetupDB(connectionString);
        }

        private string SetupDB(string baseConnectionString)
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = "free-sql-db-2905813"
            };
            string targetConnectionString = builder.ConnectionString;

            EnsureDatabase.For.SqlDatabase(targetConnectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(targetConnectionString)
                .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
                .WithTransactionPerScript()
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw result.Error;
            }
            return builder.ConnectionString;
        }
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DBMockup>
    {

    }
}
