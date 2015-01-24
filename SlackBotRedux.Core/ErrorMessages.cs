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
    }
}
