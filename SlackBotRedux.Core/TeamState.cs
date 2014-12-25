using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    /// <summary>
    /// Contains information about the state of a Slack team.
    /// Currently only contains useful information for a basic bot, such as a list of users and their ids/names.
    /// </summary>
    public class TeamState
    {
        public IDictionary<string, User> UsersById { get; set; } 
    }
}
