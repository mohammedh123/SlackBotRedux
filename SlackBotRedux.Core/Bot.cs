﻿using System;
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
        /// Adds a listener that listens for messages that match the regex exactly (as if the regex started with '^'). It will only match for messages that occur after "botname: ", "@botname", etc.
        /// </summary>
        void RespondTo(Regex regex, Action<BotMessage> callback);
    }

    public class Bot : IBotRegistrar
    {
        private TeamState _teamState;
        private readonly IList<AbstractListener> _listeners;
        private readonly string _botName;

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

        public void RespondTo(Regex regex, Action<BotMessage> callback)
        {
            var regexStr = regex.ToString();
            if(regexStr.StartsWith("^")) throw new ArgumentException("The supplied regex has an anchor in it; pass in a regex without one.");

            var newRegexPartOne = String.Format(@"^@?{0}[:,]\s+", Regex.Escape(_botName));
            var newRegexPartTwo = regexStr;
            var finalRegex = new Regex(newRegexPartOne + newRegexPartTwo);

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
