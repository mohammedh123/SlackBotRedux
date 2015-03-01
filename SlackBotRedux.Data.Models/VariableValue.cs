using System;

namespace SlackBotRedux.Data.Models
{
    public class VariableValue
    {
        public int VariableId { get; set; }
        public string Value { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}