using System.Linq;
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

                var username = textMsg.Match.Groups[1].Value;
                var textToRemember = textMsg.Match.Groups[2];

                var user = bot.TeamState.GetUserByUsername(username);
                if (user == null)
                {
                    res.Send(ErrorMessages.NoUserExistsForUsername(username));
                    return;
                }
            });
        }
    }
}
