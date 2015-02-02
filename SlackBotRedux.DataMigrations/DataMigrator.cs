using System;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;

namespace SlackBotRedux.DataMigrations
{
    public class MigrationOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }
        public string ProviderSwitches { get; set; }
        public int Timeout { get; set; }
    }

    public class DataMigrator
    {
        private readonly string _connectionString;

        public DataMigrator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void MigrateDatabase(Action<IMigrationRunner> runnerFunc)
        {
            var announcer = new NullAnnouncer();
            var migrationsAssembly = Assembly.GetExecutingAssembly();

            var migrationContext = new RunnerContext(announcer);
            var options = new MigrationOptions() { PreviewOnly = false, Timeout = 0 };
            var factory = new SqlServerProcessorFactory();
            using (var processor = factory.Create(_connectionString, announcer, options))
            {
                var runner = new MigrationRunner(migrationsAssembly, migrationContext, processor);
                runnerFunc(runner);
                processor.CommitTransaction();
            }
        }
    }
}
