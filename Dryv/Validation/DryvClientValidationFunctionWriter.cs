using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Translation;

namespace Dryv.Validation
{
    public class DryvClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        private readonly DryvOptions options;

        public DryvClientValidationFunctionWriter(DryvOptions options)
        {
            this.options = options;
        }

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

                if (!string.IsNullOrWhiteSpace(rule.Rule.Group))
                {
                    writer.Write("\"group\": \"");
                    writer.Write(this.JavaScriptEscape(rule.Rule.Group));
                    writer.Write("\",");
                }

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

                if (rule.Rule.RelatedProperties?.Any() == true)
                {
                    writer.Write("\"related\": ");
                    this.WriteArray(writer, rule.Rule.RelatedProperties.Values);
                    writer.Write(",");
                }

                writer.Write("\"validate\": ");
                writer.Write(rule.ClientCode);
                writer.Write(",\"annotations\": ");
                this.WriteObject(writer, rule.Rule.Annotations);

                writer.Write("}");
            }

            writer.Write("]");
        };

        private void WriteArray(TextWriter writer, ICollection<string> items)
        {
            var sep = string.Empty;
            writer.Write("[");

            if (items != null)
            {
                foreach (var item in items)
                {
                    writer.Write(sep);
                    writer.Write(JavaScriptHelper.TranslateValue(item) ?? this.options.JsonConversion(item));
                    sep = ",";
                }
            }

            writer.Write("]");
        }

        private void WriteObject(TextWriter writer, IDictionary<string, object> parameters)
        {
            var sep = string.Empty;
            writer.Write("{");

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    writer.Write(sep);
                    writer.Write("\"");
                    writer.Write(parameter.Key.ToCamelCase());
                    writer.Write("\":");
                    writer.Write(JavaScriptHelper.TranslateValue(parameter.Value) ?? this.options.JsonConversion(parameter.Value));
                    sep = ",";
                }
            }

            writer.Write("}");
        }
    }
}