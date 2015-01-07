using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackBotRedux.Core.Listeners;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public interface IBotRegistrar
    {
        /// <summary>
        /// Adds a listener that listens for messages that match the regex exactly (as if the regex started with '^' and ended with '$'). It will only match for messages that occur after "botname: ", "@botname", etc, and is case insensitive.
        /// </summary>
        void RespondTo(string regex, Action<BotMessage> callback);
    }

    public class Bot : IBotRegistrar
    {
        private TeamState _teamState;
        private readonly IList<AbstractListener> _listeners;
        private readonly string _botName;

        private static readonly string[] IllegalAnchors = { @"\A", @"\Z", @"\z" };

        public Bot(string botName)
        {
            _listeners = new List<AbstractListener>
            {
                new UpdateTeamListener(this),
                new UpdateUserListener(this)
            };

            _botName = botName;
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

        public void RespondTo(string regex, Action<BotMessage> callback)
        {
            if(regex.StartsWith("^")) throw new ArgumentException(String.Format("The supplied regex begins with the caret anchor; pass in a regex without one."));
            if(regex.EndsWith("$") && !regex.EndsWith(@"\$")) throw new ArgumentException(String.Format("The supplied regex ends with the dollar anchor; pass in a regex without one."));

            foreach (var illegalAnchor in IllegalAnchors) {
                if(regex.Contains(illegalAnchor)) throw new ArgumentException(String.Format("The supplied regex contains an illegal anchor; pass in a regex without the following illegal anchor: {0}", illegalAnchor));
            }
            
            var newRegexPartOne = String.Format(@"^@?{0}[:,]\s+", Regex.Escape(_botName));
            var newRegexPartTwo = regex + "$";
            var finalRegex = new Regex(newRegexPartOne + newRegexPartTwo, RegexOptions.IgnoreCase);

            _listeners.Add(new TextListener(finalRegex, callback));
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
