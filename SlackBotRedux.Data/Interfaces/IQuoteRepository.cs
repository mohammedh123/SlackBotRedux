using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Data.Interfaces
{
    public interface IQuoteRepository
    {
        Quote InsertNewQuote(string userId, string text);
    }
}
