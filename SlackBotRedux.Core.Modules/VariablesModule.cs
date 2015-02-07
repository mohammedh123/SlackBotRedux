using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Modules
{
    public class VariablesModule : IBotModule
    {
        public void RegisterToBot(IBot bot)
        {
            bot.RespondTo(@"create var [a-zA-Z]+", res =>
            {
                var textMsg = (TextInputBotMessage)res.Message;

                res.Send(ErrorMessages.RedirectCreateVar(textMsg.User.Name));
            });
        }
    }
}
