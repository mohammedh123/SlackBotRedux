using System;
using System.Text.RegularExpressions;

namespace SlackBotRedux.Core
{
    public class Response
    {
        public BotMessage Message { get; set; }
        public Match Match { get; set; }

        private readonly IBot _bot;
        
        public Response(IBot bot, BotMessage message, Match match)
        {
            _bot = bot;
            Message = message;
            Match = match;
        }

        public void Send(string msg, params object[] paramz)
        {
            var textMsg = Message as TextInputBotMessage;
            if (textMsg == null) return;

            _bot.Send(textMsg.Message.Channel, String.Format(msg, paramz));
        }
    }
}
