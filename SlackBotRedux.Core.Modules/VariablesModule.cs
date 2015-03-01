using System;
using SlackBotRedux.Configuration;
using SlackBotRedux.Core.Variables;
using SlackBotRedux.Core.Variables.Interfaces;

namespace SlackBotRedux.Core.Modules
{
    public class VariablesModule : IBotModule
    {
        private readonly IVariableConfiguration _config;
        private readonly IVariableDictionary _variableDictionary;

        public VariablesModule(IVariableConfiguration config, IVariableDictionary variableDictionary)
        {
            _config = config;
            _variableDictionary = variableDictionary;
        }

        public void RegisterToBot(IBot bot)
        {
            bot.RespondTo(@"create var .+", res =>
            {
                var textMsg = (TextInputBotMessage) res.Message;

                res.Send(ErrorMessages.RedirectCreateVar(textMsg.User.Name));
            });

            var addValueRegex = String.Format(@"add value (?<Value>{0}+) (?<Name>{1}+)",
                                              _config.AllowedValueCharactersRegex,
                                              _config.AllowedNameCharactersRegex);

            bot.RespondTo(addValueRegex, res =>
            {
                var textMsg = (TextInputBotMessage) res.Message;

                var username = textMsg.User.Name;
                var varName = textMsg.Match.Groups["Name"].Value;
                var varValue = textMsg.Match.Groups["Value"].Value;

                var varDef = _variableDictionary.GetVariable(varName);
                if (varDef == null) {
                    _variableDictionary.AddVariable(varName, false);
                }

                var addResult = _variableDictionary.TryAddValue(varName, varValue);
                res.Send(GetMessageFromAddResult(username, varName, varValue, addResult));
            });
        }

        private string GetMessageFromAddResult(string username, string name, string value, TryAddValueResult result)
        {
            switch (result.Result)
            {
                case TryAddValueResultEnum.Success:
                    return SuccessMessages.SuccessfulAddValueToVariable(username);
                case TryAddValueResultEnum.VariableDoesNotExist:
                    throw new InvalidOperationException("Got a VariableDoesNotExist, but this should not be possible because the variable should be automatically created if it doesn't exist.");
                case TryAddValueResultEnum.ValueAlreadyExists:
                    return ErrorMessages.VariableValueAlreadyExists(username, name, value);
                case TryAddValueResultEnum.VariableIsProtected:
                    return ErrorMessages.CantModifyProtectedVariable(username, name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
