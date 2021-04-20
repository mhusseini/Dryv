using System;
using System.Globalization;

namespace Dryv.Translation
{
    public static class JavaScriptHelper
    {
        public static string TranslateValue(object value)
        {
            return value switch
            {
                string txt => $"\"{JavaScriptEscape(txt)}\"",
                Enum _ => null,
                bool b => (b ? "true" : "false"),
                null => "null",
                DateTime dateTime => $@"""{dateTime.ToString(CultureInfo.CurrentUICulture)}""",
                DateTimeOffset dateTime => $@"""{dateTime.ToString(CultureInfo.CurrentUICulture)}""",
                _ => value.ToString()
            };
        }

        public static string JavaScriptEscape(string text)
        {
            return text
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\\", @"\u005c")  // Because it's JS string escape character
                .Replace("\"", @"\u0022")  // Because it may be string delimiter
                .Replace("'", @"\u0027")   // Because it may be string delimiter
                .Replace("&", @"\u0026")   // Because it may interfere with HTML parsing
                .Replace("<", @"\u003c")   // Because it may interfere with HTML parsing
                .Replace(">", @"\u003e");  // Because it may interfere with HTML parsing
        }
    }
}