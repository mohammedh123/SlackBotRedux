using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public abstract class AbstractListener : AbstractListener<BotMessage>
    {
        protected AbstractListener(Func<BotMessage, bool> matcher, Func<BotMessage, bool> callback) : base(matcher, callback)
        {}
    }

    public abstract class AbstractListener<TMessageType> where TMessageType : BotMessage
    {
        protected Func<TMessageType, bool> Matcher;
        protected Func<TMessageType, bool> Callback;

        protected AbstractListener()
        {}

        protected AbstractListener(Func<TMessageType, bool> matcher, Func<TMessageType, bool> callback)
        {
            Matcher = matcher;
            Callback = callback;
        }

        public bool Listen(TMessageType msg)
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

    public class TextListener : AbstractListener<TextInputBotMessage>
    {
        private readonly Regex _regex;

        public TextListener(Regex regex, Func<TextInputBotMessage, bool> callback)
        {
            _regex = regex;
            Matcher = tibm =>
            {
                var match = _regex.Match(tibm.Message.Text);
                if (!match.Success) return false;
                
                tibm.Match = match;
                return true;
            };
            Callback = callback;
        }
    }
}
