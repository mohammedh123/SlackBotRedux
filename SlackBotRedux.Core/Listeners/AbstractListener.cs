using System;

namespace SlackBotRedux.Core.Listeners
{
    public abstract class AbstractListener
    {
        protected Func<BotMessage, bool> Matcher;
        protected Action<BotMessage> Callback;

        protected AbstractListener()
        { }

        protected AbstractListener(Func<BotMessage, bool> matcher, Action<BotMessage> callback)
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
}
