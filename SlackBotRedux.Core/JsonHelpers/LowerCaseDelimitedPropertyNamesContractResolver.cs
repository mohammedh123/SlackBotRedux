using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace SlackBotRedux.Core.JsonHelpers
{
    // http://www.vicesoftware.com/uncategorized/extending-json-net-to-serialize-json-properties-using-a-format-that-is-delimited-by-dashes-and-all-lower-case/
    public class LowerCaseDelimitedPropertyNamesContractResolver : DefaultContractResolver
    {
        private readonly char _delimiter;

        public LowerCaseDelimitedPropertyNamesContractResolver(char delimiter)
            : base(true)
        {
            _delimiter = delimiter;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToDelimitedString(_delimiter);
        }
    }

    public static class StringExtensions
    {
        public static string ToDelimitedString(this string @string, char delimiter)
        {
            var camelCaseString = @string.ToCamelCaseString();
            return new string(InsertDelimiterBeforeCaps(camelCaseString, delimiter).ToArray());
        }

        public static string ToCamelCaseString(this string @string)
        {
            if (string.IsNullOrEmpty(@string) || !char.IsUpper(@string[0]))
            {
                return @string;
            }
            string lowerCasedFirstChar =
                char.ToLower(@string[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            if (@string.Length > 1)
            {
                lowerCasedFirstChar = lowerCasedFirstChar + @string.Substring(1);
            }
            return lowerCasedFirstChar;
        }

        private static IEnumerable<char> InsertDelimiterBeforeCaps(IEnumerable<char> input, char delimiter)
        {
            bool lastCharWasUppper = false;
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    if (!lastCharWasUppper)
                    {
                        yield return delimiter;
                        lastCharWasUppper = true;
                    }
                    yield return char.ToLower(c);
                    continue;
                }

                yield return c;
                lastCharWasUppper = false;
            }
        }
    }
}
