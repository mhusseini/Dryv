using System;
using System.Collections.Generic;
using System.IO;

namespace Dryv.Validation
{
    public class DryvClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        private string JavaScriptEscape(string text)
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

        public Action<TextWriter> GetValidationFunction(IEnumerable<TranslatedRule> translatedRules) => writer =>
        {
            var sep = string.Empty;
            writer.Write("[");

            foreach (var rule in translatedRules)
            {
                writer.Write(sep);
                sep = ",";
                writer.Write("{");

                if (!string.IsNullOrWhiteSpace(rule.Rule.Name))
                {
                    writer.Write("\"name\": \"");
                    writer.Write(this.JavaScriptEscape(rule.Rule.Name));
                    writer.Write("\",");
                }

                if (rule.Rule.IsAsync)
                {
                    writer.Write("\"async\": true,");
                }

                writer.Write("\"validate\": ");
                writer.Write(rule.ClientCode);

                writer.Write("}");
            }

            writer.Write("]");
        };
    }
}