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
                string txt => $"\"{txt}\"",
                bool b => (b ? "true" : "false"),
                null => "null",
                DateTime dateTime => $@"""{dateTime.ToString(CultureInfo.CurrentCulture)}""",
                DateTimeOffset dateTime => $@"""{dateTime.ToString(CultureInfo.CurrentCulture)}""",
                _ => null
            };
        }
    }
}