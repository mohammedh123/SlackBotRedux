using System;
using System.Text.RegularExpressions;

namespace SlackBotRedux.Core.Listeners
{
    public class TextListener : AbstractListener
    {
        private readonly Regex _regex;

        public TextListener(Regex regex, Action<BotMessage> callback)
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
}