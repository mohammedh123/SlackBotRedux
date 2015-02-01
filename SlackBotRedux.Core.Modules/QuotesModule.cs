using System;
using System.Linq;
using SlackBotRedux.Data.Interfaces;

namespace SlackBotRedux.Core.Modules
{
    public class QuotesModule : IBotModule
    {
        private readonly IRecentMessageRepository _recentMsgRepository;
        private readonly IQuoteRepository _quoteRepository;

        public QuotesModule(IRecentMessageRepository recentMsgRepository, IQuoteRepository quoteRepository)
        {
            _recentMsgRepository = recentMsgRepository;
            _quoteRepository = quoteRepository;
        }

        public void RegisterToBot(IBot bot)
        {
            bot.RespondTo(@"remember (\w+) (.+)", res =>
            {
                var textMsg = (TextInputBotMessage)res.Message;

                var username = textMsg.User.Name;
                var targetName = textMsg.Match.Groups[1].Value;
                var textToRemember = textMsg.Match.Groups[2].Value;

                var targetUser = bot.TeamState.GetUserByUsername(targetName);
                if (targetUser == null) {
                    res.Send(ErrorMessages.NoUserExistsForUsername(username, targetName));
                    return;
                }

                if (String.Equals(textMsg.Message.User, targetUser.Id)) {
                    res.Send(ErrorMessages.CantUseSelfAsTarget(username, "quote"));
                    return;
                }

                var matchingMsg = _recentMsgRepository.GetRecentMessagesByUserId(targetUser.Id).FirstOrDefault(msg => msg.Text.Contains(textToRemember));
                if (matchingMsg == null) {
                    res.Send(ErrorMessages.NoQuotesForUser(username, targetName, textToRemember));
                    return;
                }

                _quoteRepository.InsertNewQuote(targetUser.Id, matchingMsg.Text);

                res.Send(SuccessMessages.SuccessfulRemember(username, matchingMsg.Text));
            });
        }
    }
}
