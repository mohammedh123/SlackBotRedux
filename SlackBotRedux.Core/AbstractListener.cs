using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public abstract class AbstractListener
    {
        protected Func<BotMessage, bool> Matcher;
        protected Func<BotMessage, bool> Callback;

        protected AbstractListener()
        { }

        protected AbstractListener(Func<BotMessage, bool> matcher, Func<BotMessage, bool> callback)
        {
            Matcher = matcher;
            Callback = callback;
        }

        public bool Listen(BotMessage msg)
        {
            if (Matcher(msg)) {
                Callback(msg);

                return true;
            }
            else {
                return false;
            }
        }
    }

    public class TextListener : AbstractListener
    {
        private readonly Regex _regex;

        public TextListener(Regex regex, Func<BotMessage, bool> callback)
        {
            _regex = regex;
            Matcher = msg =>
            {
                var tibm = msg as TextInputBotMessage;
                if (tibm == null) return false;

                var match = _regex.Match(tibm.Message.Text);
                if (!match.Success) return false;
                
                tibm.Match = match;
                return true;
            };
            Callback = callback;
        }
    }

    public class UpdateTeamListener : AbstractListener
    {
        public UpdateTeamListener(Bot bot)
            : base(msg => msg is UpdateTeamBotMessage,
            msg => ProcessMessage(bot, msg))
        { }

        private static bool ProcessMessage(Bot bot, BotMessage msg)
        {
            var updateTeamMsg = (UpdateTeamBotMessage)msg;
            bot.UpdateTeamState(updateTeamMsg.TeamState);

            return true;
        }
    }

    public class UpdateUserListener : AbstractListener
    {
        public UpdateUserListener(Bot bot)
            : base(msg => msg is UpdateUserBotMessage,
            msg => ProcessMessage(bot, msg))
        { }

        private static bool ProcessMessage(Bot bot, BotMessage msg)
        {
            var updateUserMsg = (UpdateUserBotMessage)msg;
            bot.UpsertUserInTeamState(updateUserMsg.User);

            return true;
        }
    }
}
