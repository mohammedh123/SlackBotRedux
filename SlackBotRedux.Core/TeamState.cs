using System;
using System.Collections.Generic;
using System.Linq;
using SlackBotRedux.Core.Models.Slack;

namespace SlackBotRedux.Core
{
    public interface ITeamState
    {
        User GetUserByUsername(string username);
        User GetUserByUserId(string userId);
        void UpsertUser(string userId, User user);
    }

    /// <summary>
    /// Contains information about the state of a Slack team.
    /// Currently only contains useful information for a basic bot, such as a list of users and their ids/names.
    /// </summary>
    public class TeamState : ITeamState
    {
        internal IDictionary<string, User> UsersById { get; set; }

        public User GetUserByUsername(string username)
        {
            return UsersById.FirstOrDefault(kvp => String.Equals(kvp.Value.Name, username)).Value;
        }

        public User GetUserByUserId(string userId)
        {
            User user;
            UsersById.TryGetValue(userId, out user);

            return user;
        }

        public void UpsertUser(string userId, User user)
        {
            if (UsersById.ContainsKey(user.Id)) {
                UsersById[user.Id] = user;
            }
            else {
                UsersById.Add(user.Id, user);
            }
        }
    }
}
