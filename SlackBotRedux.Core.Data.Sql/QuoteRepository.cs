using System;
using System.Data;
using Dapper.Contrib.Extensions;
using SlackBotRedux.Core.Data.Interfaces;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core.Data.Sql
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly IDbConnection _conn;

        public QuoteRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public Quote InsertNewQuote(string userId, string text)
        {
            var quote = new Quote {Text = text, Timestamp = DateTimeOffset.UtcNow, UserId = userId};
            quote.Id = (int)_conn.Insert(quote);

            return quote;
        }
    }
}
