using System.Linq;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.Core.Models;
using SlackBotRedux.Data.Sql;

namespace SlackBotRedux.Tests.Data.Sql
{
    [TestClass]
    internal class TQuoteRepository : DatabaseTest
    {
        protected QuoteRepository Subject;

        [TestInitialize]
        public void InitializeSubject()
        {
            Subject = new QuoteRepository(Connection);
        }

        [TestClass]
        public class InsertNewQuote : TQuoteRepository
        {
            [TestMethod]
            public void ShouldSucceedInsertingQuote()
            {
                // Act
                const string userId = "userId";
                const string quoteText = "text goes here";
                Subject.InsertNewQuote(userId, quoteText);

                // Verify
                var quote = Connection.Query<Quote>("SELECT * FROM dbo.Quotes WHERE UserId = @userId", new {userId}).SingleOrDefault();
                quote.Should().NotBeNull();
                quote.UserId.Should().Be(userId);
                quote.Text.Should().Be(quoteText);
            }
        }
    }
}
