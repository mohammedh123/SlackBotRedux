using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.DataMigrations;

namespace SlackBotRedux.Tests
{
    [TestClass]
    internal abstract class DatabaseTest
    {
        protected string ConnectionString;
        protected IDbConnection Connection;
        protected TransactionScope Transaction;

        private DataMigrator _migrator;

        [TestInitialize]
        public void Setup()
        {
            // necessary for connection string location
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            PopulateConnectionString();
            _migrator = new DataMigrator(ConnectionString);
            InitializeDatabase();
            InitializeConnection();
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupConnection();
            CleanupDatabase();
        }

        private void PopulateConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings["TestDb"];
            if (connStr == null) throw new ArgumentException("You forgot to populate a connection string the test App.config with key=\"TestDb\".");
            ConnectionString = connStr.ConnectionString;
        }

        private void InitializeDatabase()
        {
            _migrator.MigrateDatabase(mr => mr.MigrateDown(0));
            _migrator.MigrateDatabase(mr => mr.MigrateUp());
        }

        private void InitializeConnection()
        {
            Transaction = new TransactionScope();
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
        }

        private void CleanupConnection()
        {
            Connection.Dispose();
            Transaction.Dispose();
        }

        private void CleanupDatabase()
        {
            _migrator.MigrateDatabase(mr => mr.MigrateDown(0));    
        }
    }
}
