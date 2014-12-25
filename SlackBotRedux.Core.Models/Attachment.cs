using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core.Models
{
    public class Attachment
    {
        public string Fallback { get; set; }

        public string Text { get; set; }
        public string PreText { get; set; }

        public string Color { get; set; }

        public class Field
        {
            public string Title { get; set; }
            public string Value { get; set; }
            public bool Short { get; set; }
        }
        public List<Field> Fields { get; set; }
    }
}
