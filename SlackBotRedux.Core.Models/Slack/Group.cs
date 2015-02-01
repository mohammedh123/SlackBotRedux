using System.Collections.Generic;

namespace SlackBotRedux.Core.Models.Slack
{
    /// <summary>
    /// Contains information about a Slack group.
    /// More information can be found at https://api.slack.com/types/group.
    /// </summary>
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public long Created { get; set; }
        public string Creator { get; set; }
        public bool IsArchived { get; set; }
        public List<string> Members { get; set; }
        public Topic Topic { get; set; }
        public Purpose Purpose { get; set; }

        public bool IsOpen { get; set; }
        public long LastRead { get; set; }
        public int UnreadCount { get; set; }
        public InputMessage Latest { get; set; }
    }
}
