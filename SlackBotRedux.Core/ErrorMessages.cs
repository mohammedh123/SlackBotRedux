using System;

namespace SlackBotRedux.Core
{
    public static class ErrorMessages
    {
        public static string NoUserExistsForUsername(string username)
        {
            return String.Format("Sorry {0}, but no user exists with that username.", username);
        }
    }
}
