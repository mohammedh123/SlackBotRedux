using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SlackBotRedux.Core.Listeners;

namespace SlackBotRedux.Core
{
    public interface IBot
    {
        ITeamState TeamState { get; set; }

        /// <summary>
        /// Adds a listener that listens for messages that match the regex exactly (as if the regex started with '^' and ended with '$'). It will only match for messages that occur after "botname: ", "@botname", etc, and is case insensitive.
        /// </summary>
        void RespondTo(string regex, Action<Response> callback);

        void Send(string channel, string msg);
    }
    
    public class Bot : IBot
    {
        public ITeamState TeamState { get; set; }

        private readonly IList<AbstractListener> _listeners;
        private readonly string _botName;
        private readonly IMessageSender _pipeline;

        private static readonly string[] IllegalAnchors = { @"\A", @"\Z", @"\z" };

        public Bot(string botName, IMessageSender pipeline)
        {
            _listeners = new List<AbstractListener>
            {
                new UpdateTeamListener(this),
                new UpdateUserListener(this)
            };

            _botName = botName;
            _pipeline = pipeline;
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
        
        public void RespondTo(string regex, Action<Response> callback)
        {
            if(regex.StartsWith("^")) throw new ArgumentException(String.Format("The supplied regex begins with the caret anchor; pass in a regex without one."));
            if(regex.EndsWith("$") && !regex.EndsWith(@"\$")) throw new ArgumentException(String.Format("The supplied regex ends with the dollar anchor; pass in a regex without one."));

            foreach (var illegalAnchor in IllegalAnchors) {
                if(regex.Contains(illegalAnchor)) throw new ArgumentException(String.Format("The supplied regex contains an illegal anchor; pass in a regex without the following illegal anchor: {0}", illegalAnchor));
            }
            
            var newRegexPartOne = String.Format(@"^@?{0}[:,]\s+", Regex.Escape(_botName));
            var newRegexPartTwo = regex + "$";
            var finalRegex = new Regex(newRegexPartOne + newRegexPartTwo, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            _listeners.Add(new TextListener(this, finalRegex, callback));
        }

        public void Send(string channel, string msg)
        {
            _pipeline.EnqueueOutputMessage(channel, msg);
        }
    }

    internal class UpdateTeamListener : AbstractListener
    {
        public UpdateTeamListener(IBot bot)
            : base(bot, msg => msg is UpdateTeamBotMessage,
            msg => ProcessMessage(bot, msg.Message))
        { }

        private static bool ProcessMessage(IBot bot, BotMessage msg)
        {
            var updateTeamMsg = (UpdateTeamBotMessage)msg;
            bot.TeamState = updateTeamMsg.TeamState;

            return true;
        }
    }

    internal class UpdateUserListener : AbstractListener
    {
        public UpdateUserListener(IBot bot)
            : base(bot, msg => msg is UpdateUserBotMessage,
            msg => ProcessMessage(bot, msg.Message))
        { }

        private static bool ProcessMessage(IBot bot, BotMessage msg)
        {
            var updateUserMsg = (UpdateUserBotMessage)msg;
            bot.TeamState.UpsertUser(updateUserMsg.User.Id, updateUserMsg.User);

            return true;
        }
    }
}
