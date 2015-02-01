using System.Collections.Generic;

namespace SlackBotRedux.Core.Models.Slack
{
    /// <summary>
    /// Contains information about a Slack team channel.
    /// More information can be found at https://api.slack.com/types/channel.
    /// </summary>
    public class Channel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsChannel { get; set; }

        /// <summary>
        /// The date the channel was created (Unix timestamp).
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        /// The id of the user who created the channel.
        /// </summary>
        public string Creator { get; set; }

        public bool IsArchived { get; set; }
        public bool IsGeneral { get; set; }

        /// <summary>
        /// A list of user ids for all users in this channel (includes any disabled accounts that were in this channel when they were disabled).
        /// </summary>
        public List<string> Members { get; set; }

        public Topic Topic { get; set; }

        public Purpose Purpose { get; set; }

        public bool IsMember { get; set; }

        public string LastRead { get; set; }
        public List<InputMessage> Latest { get; set; }
        public int UnreadCount { get; set; }
    }
}
