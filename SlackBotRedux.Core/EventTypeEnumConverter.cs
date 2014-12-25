using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public class EventTypeEnumConverter : StringEnumConverter
    { 
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string) reader.Value;

            var enumVals = Enum.GetNames(typeof(EventType));
            var match =
                enumVals.FirstOrDefault(
                    str => str.Equals(enumString.Replace("_", String.Empty), StringComparison.InvariantCultureIgnoreCase));
            if (match == null) {
                return null;
            }

            return Enum.Parse(typeof(EventType), match);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EventType);
        }
    }
}
