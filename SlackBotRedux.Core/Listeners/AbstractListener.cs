using System;
using System.Text.RegularExpressions;

namespace SlackBotRedux.Core.Listeners
{
    public abstract class AbstractListener
    {
        private readonly IBot _bot;
        protected Func<BotMessage, bool> Matcher;
        protected Action<Response> Callback;

        protected AbstractListener(IBot bot)
        {
            _bot = bot;
        }

        protected AbstractListener(IBot bot, Func<BotMessage, bool> matcher, Action<Response> callback) : this(bot)
        {
            Matcher = matcher;
            Callback = callback;
        }

        public bool Listen(BotMessage msg)
        {
            var match = Matcher(msg);
            if (match) {
                Callback(new Response(_bot, msg, msg.Match));

                return true;
            }
            else {
                return false;
            }
        }
    }
}
