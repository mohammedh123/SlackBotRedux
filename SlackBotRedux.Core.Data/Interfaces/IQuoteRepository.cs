using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core.Data.Interfaces
{
    public interface IQuoteRepository
    {
        Quote InsertNewQuote(string userId, string text);
    }
}
