using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public class Bot
    {
        private TeamState _teamState;
        private readonly IList<AbstractListener> _listeners;

        public Bot()
        {
            _listeners = new List<AbstractListener>
            {
                new UpdateTeamListener(this),
                new UpdateUserListener(this)
            };
        }

        public void ReceiveMessage(BotMessage msg)
        {
            var results = new List<bool>();
            foreach (var listener in _listeners) {
                results.Add(listener.Listen(msg));

                if (msg.IsFinishedBeingProcessed) break;
            }

            // if none of the listeners care for this message (and its not a fallback msg), then update the msg type to a Fallback message
            if (!results.Any(b => b) && !(msg is FallbackMessage)) {
                ReceiveMessage(new FallbackMessage(msg));
            }
        }

        internal void UpdateTeamState(TeamState teamState)
        {
            _teamState = teamState;
        }

        internal void UpsertUserInTeamState(User user)
        {
            if (_teamState.UsersById.ContainsKey(user.Id)) {
                _teamState.UsersById[user.Id] = user;
            }
            else {
                _teamState.UsersById.Add(user.Id, user);
            }
        }
    }

    internal class UpdateTeamListener : AbstractListener
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

    internal class UpdateUserListener : AbstractListener
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
