using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Models
{
    public class OutputMessage
    {
        public uint Id { get; set; }
        public string Type { get { return "message"; } }
        public string Channel { get; set; }
        public string Text { get; set; }

        public OutputMessage(uint id, string channel, string text)
        {
            Id = id;
            Channel = channel;
            Text = text;
        }
    }
}
