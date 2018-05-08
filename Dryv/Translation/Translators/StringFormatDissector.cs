using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dryv.Translation.Translators
{
    internal class StringFormatDissector
    {
        private static readonly Regex RegexEscaped = new Regex(@"\{{2,}\d+(:.+?)?\}{2,}");
        private static readonly Regex RegexUnescaped = new Regex(@"\{{1,1}(\d+)(:.+?)?\}{1,1}");

        public IList<object> Parse(string pattern)
        {
            var matches = RegexEscaped.Matches(pattern).Cast<Match>().ToList();
            var matches2 = (from match in RegexUnescaped.Matches(pattern).Cast<Match>()
                            where !matches.Any(m => m.Index <= match.Index && m.Index + m.Length >= match.Index + match.Length)
                            select match).ToList();
            var result = new List<object>();
            var index = 0;
            foreach (var match in matches2)
            {
                result.Add(pattern.Substring(index, match.Index - index));
                result.Add(int.Parse(match.Groups[1].Value));
                index = match.Index + match.Length;
            }

            result.Add(pattern.Substring(index, pattern.Length - index));
            return result;
        }

        public IList<object> Recombine(string pattern, object[] arguments)
        {
            var result = new List<object>();

            foreach (var part in this.Parse(pattern))
            {
                switch (part)
                {
                    case string text:
                        result.Add(text);
                        break;
                    case int index:
                        result.Add(arguments[index]);
                        break;
                }
            }

            return result;
        }
    }
}