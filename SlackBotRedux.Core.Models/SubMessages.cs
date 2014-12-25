using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Models
{
    public class UserChangeMessage
    {
        public EventType Type { get; set; }

        public User User { get; set; }
    }
}
