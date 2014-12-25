using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Models
{
    public class RtmStartResponse
    {
        public bool Ok { get; set; }

        /// <summary>
        /// The WebSocket Message Server URL; connecting to this URL will initiate a Real Time Messaging session. These URLs are only valid for 30 seconds.
        /// </summary>
        public string Url { get; set; }

        public class SelfData
        {
            public string Id { get; set; }
            public string Name { get; set; }

            /// <summary>
            /// User-specific preferences.
            /// </summary>
            public Dictionary<string, string> Prefs { get; set; }

            /// <summary>
            /// The date the user was created (Unix timestamp).
            /// </summary>
            public long Created { get; set; }

            public string ManualPresence { get; set; }
        }
        public SelfData Self { get; set; }

        public class TeamData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string EmailDomain { get; set; }
            public string Domain { get; set; }
            public int MsgEditWindowMins { get; set; }
            public bool OverStorageLimit { get; set; }

            /// <summary>
            /// Team-specific preferences.
            /// </summary>
            public Dictionary<string, string> Prefs { get; set; }
        }
        public TeamData Team { get; set; }

        public List<User> Users { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Group> Groups { get; set; }
        public List<DirectMessageChannel> Ims { get; set; }
    }
}
