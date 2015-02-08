using System;
using System.Collections.Generic;

namespace SlackBotRedux.Data.Models
{
    public class Variable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsProtected { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}
