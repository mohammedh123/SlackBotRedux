using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlackBotRedux.Tests
{
    [TestClass]
    internal abstract class DatabaseTest
    {
        protected IDbConnection Connection;
        protected TransactionScope Transaction;

        [TestInitialize]
        public void InitializeConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["TestDb"];
            if(connStr == null) throw new ArgumentException("You forgot to populate a connection string the test App.config with key=\"TestDb\".");

            Transaction = new TransactionScope();
            Connection = new SqlConnection(connStr.ConnectionString);
            Connection.Open();
        }

        [TestCleanup]
        public void CleanupConnection()
        {
            Connection.Dispose();
            Transaction.Dispose();
        }
    }
}
