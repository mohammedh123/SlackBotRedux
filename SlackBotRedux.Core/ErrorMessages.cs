using System;

namespace SlackBotRedux.Core
{
    public static class ErrorMessages
    {
        public static string NoUserExistsForUsername(string username, string nonexistentUsername)
        {
            return String.Format("Sorry {0}, but I don't know anyone named {1}.", username, nonexistentUsername);
        }

        public static string CantUseSelfAsTarget(string username, string command)
        {
            return String.Format("Sorry {0}, but you can't {1} yourself.", username, command);
        }

        public static string NoQuotesForUser(string username, string quotedUser, string textToRemember)
        {
            return String.Format("Sorry {0}, but I don't remember what {1} said about \"{2}\".", username, quotedUser, textToRemember);
        }

        public static string RedirectCreateVar(string username)
        {
            return String.Format("{0}: To create a variable, just start adding values to it.", username);
        }

        public static string VariableValueAlreadyExists(string username, string variableName, string variableValue)
        {
            return String.Format("Okay {0}, but {1} already had \"{2}\" as a value.", username, variableName, variableValue);
        }

        public static string VariableDoesNotExist(string username, string variableName)
        {
            return String.Format("Okay {0}, but no variable with the name \"{1}\" exists.", username, variableName);
        }

        public static string VariableValueDoesNotExist(string username, string variableName, string variableValue)
        {
            return String.Format("Okay {0}, but {1} does not have \"{2}\" as a value.", username, variableName, variableValue);
        }

        public static string CantModifyProtectedVariable(string username, string variableName)
        {
            return String.Format("Sorry {0}, but {1} is protected and cannot be modified.", username, variableName);
        }
    }
}
