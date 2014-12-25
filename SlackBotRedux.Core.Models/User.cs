using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Models
{
    /// <summary>
    /// Contains information about a Slack team member.
    /// More information can be found at https://api.slack.com/types/user.
    /// </summary>
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Color { get; set; }

        public class ProfileData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string RealName { get; set; }

            public string Email { get; set; }
            public string Skype { get; set; }
            public string Phone { get; set; }

            public string Image24 { get; set; }
            public string Image32 { get; set; }
            public string Image48 { get; set; }
            public string Image72 { get; set; }
            public string Image192 { get; set; }
        }
        public ProfileData Profile { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPrimaryOwner { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsUltraRestricted { get; set; }
        public bool HasFiles { get; set; }
    }
}
