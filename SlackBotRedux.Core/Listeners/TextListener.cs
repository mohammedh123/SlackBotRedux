using System;
using System.Text.RegularExpressions;

namespace SlackBotRedux.Core.Listeners
{
    public class TextListener : AbstractListener
    {
        private readonly Regex _regex;

        public TextListener(IBot bot, Regex regex, Action<Response> callback) : base(bot)
        {
            _regex = regex;
            Matcher = msg =>
            {
                var tibm = msg as TextInputBotMessage;
                if (tibm == null) return false;

                var match = _regex.Match(tibm.Message.Text);
                tibm.Match = match;
                return match.Success;
            };
            Callback = callback;
        }
    }
}