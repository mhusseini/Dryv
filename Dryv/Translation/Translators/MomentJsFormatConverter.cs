using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dryv.Translation.Translators
{
    /// <summary>
    /// Thank you Dimitar Nanov -MomentJsFormatConverter-Nanov.cs - https://gist.github.com/nanov/2c3fd62997fb15d2cc59dd3fc2ff6f3a
    /// </summary>
    internal static class MomentJsFormatConverter
    {
        private static readonly Regex KeywordMatchRegex;
        private static readonly Regex RegexEscapeBackslashes = new Regex(@"(\\)'");
        private static readonly Regex RegexEscapeQuotes = new Regex(@"(?<!\\)'((?:[^'\\]|\\.)*)'");
        private static readonly Dictionary<string, string> TokenDifferenceLookup;

        static MomentJsFormatConverter()
        {
            TokenDifferenceLookup = new Dictionary<string, string>()
            {
                // The day of the month, from 1 through 31.
                {"d", "D"},
                // The day of the month, from 01 through 31.
                {"dd", "DD"},
                // The tenths of a second in a date and time value.
                {"f", "S"},
                // The hundredths of a second in a date and time value.
                {"ff", "SS"},
                // The milliseconds in a date and time value.
                {"fff", "SSS"},
                // The year, from 00 to 99.
                {"yy", "YY"},
                // The year as a four-digit number.
                {"yyyy", "YYYY"},
                // Hours and minutes offset from UTC.
                {"zzz", "Z"}
            };

            var tokens = string.Join("|", TokenDifferenceLookup.OrderByDescending(t => t.Key.Length).Select(r => r.Key).ToArray());

            KeywordMatchRegex = new Regex($@"\\'|'(?:\\'|[^'])*'|({tokens})");
        }

        public static string ConvertFormat(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format));
            }

            var replacedFormat = ReplaceTokens(format);

            replacedFormat = RegexEscapeQuotes.Replace(replacedFormat, "[$1]");
            replacedFormat = RegexEscapeBackslashes.Replace(replacedFormat, string.Empty);

            return replacedFormat;
        }

        private static string ReplaceTokens(string format) =>
            KeywordMatchRegex.Replace(format, m =>
                m.Groups.Count < 2 || string.IsNullOrEmpty(m.Groups[1].Value)
                    ? m.Value
                    : TokenDifferenceLookup.ContainsKey(m.Groups[1].Value)
                        ? TokenDifferenceLookup[m.Groups[0].Value]
                        : m.Value);
    }
}