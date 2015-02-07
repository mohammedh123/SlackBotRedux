using System;
using System.Data;
using Dapper.Contrib.Extensions;
using SlackBotRedux.Data.Interfaces;
using SlackBotRedux.Data.Models;

namespace SlackBotRedux.Data.Sql
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
