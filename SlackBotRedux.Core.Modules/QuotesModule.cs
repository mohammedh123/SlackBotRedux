using System;
using System.Linq;
using System.Text.RegularExpressions;
using SlackBotRedux.Core.Data;

namespace SlackBotRedux.Core.Modules
{
    public class QuotesModule : IBotModule
    {
        private readonly IRecentMessageRepository _recentMsgRepository;

        public QuotesModule(IRecentMessageRepository recentMsgRepository)
        {
            _recentMsgRepository = recentMsgRepository;
        }

        public void RegisterToBot(IBot bot)
        {
            bot.RespondTo(@"remember (\w+) (.+)", res =>
            {
                var textMsg = (TextInputBotMessage)res.Message;

                var targetName = textMsg.Match.Groups[1].Value;
                var textToRemember = textMsg.Match.Groups[2].Value;

                var targetUser = bot.TeamState.GetUserByUsername(targetName);
                if (targetUser == null) {
                    res.Send(ErrorMessages.NoUserExistsForUsername(textMsg.User.Name, targetName));
                    return;
                }

                if (String.Equals(textMsg.Message.User, targetUser.Id)) {
                    res.Send(ErrorMessages.CantUseSelfAsTarget(textMsg.User.Name, "quote"));
                    return;
                }

                var matchingMsg = _recentMsgRepository.GetRecentMessagesByUserId(targetUser.Id).FirstOrDefault(msg => msg.Text.Contains(textToRemember));
                if (matchingMsg == null) {
                    res.Send(ErrorMessages.NoQuotesForUser(textMsg.User.Name, targetName, textToRemember));
                    return;
                }
            });
        }
    }
}
