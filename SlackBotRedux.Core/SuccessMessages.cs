using System;

namespace SlackBotRedux.Core
{
    public static class SuccessMessages
    {
        public static string SuccessfulRemember(string username, string textToBeRemembered)
        {
            return String.Format("Okay, {0}, remembering \"{1}.\"", username, textToBeRemembered);
        }

        public static string SuccessfulAddValueToVariable(string username)
        {
            return String.Format("Okay, {0}.", username);
        }
    }
}
