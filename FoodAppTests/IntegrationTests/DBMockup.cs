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

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();
            await _container.StartAsync();
            string connectionString = _container.GetConnectionString();
            SetupDB(connectionString);
        }

        private void SetupDB(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
                .WithTransactionPerScript()
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();
        }
    }
}
