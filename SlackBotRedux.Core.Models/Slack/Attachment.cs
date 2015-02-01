using System.Collections.Generic;

namespace SlackBotRedux.Core.Models.Slack
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
