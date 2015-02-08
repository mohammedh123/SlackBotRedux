using System;

namespace SlackBotRedux.Core.Modules
{
    public class VariablesModule : IBotModule
    {
        private const string AllowedVariableNameCharacters = @"[a-zA-Z0-9_-]";
        private const string AllowedVariableValueCharacters = @".";

        public void RegisterToBot(IBot bot)
        {
            bot.RespondTo(@"create var .+", res =>
            {
                var textMsg = (TextInputBotMessage) res.Message;

                res.Send(ErrorMessages.RedirectCreateVar(textMsg.User.Name));
            });
        }
    }
}
