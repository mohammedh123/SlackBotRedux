using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Models
{
    /// <summary>
    /// Contains information about a Slack direct message channel.
    /// More information can be found at https://api.slack.com/types/im.
    /// </summary>
    public class DirectMessageChannel
    {
        public string Id { get; set; }
        public bool IsIm { get; set; }

        /// <summary>
        /// The id of the other user of this direct message channel.
        /// </summary>
        public string User { get; set; }

        public long Created { get; set; }
        public bool IsUserDeleted { get; set; }

        public bool IsOpen { get; set; }
        public string LastRead { get; set; }
        public int UnreadCount { get; set; }
        public InputMessage Latest { get; set; }
    }
}
