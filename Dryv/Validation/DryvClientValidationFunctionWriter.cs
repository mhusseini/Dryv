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

        public Action<TextWriter> GetValidationFunction(IEnumerable<TranslatedRule> translatedRules) => writer =>
        {
            var keyWriter = new KeyWriter(writer);
            var sep = string.Empty;
            writer.Write("[");

            foreach (var rule in translatedRules)
            {
                writer.Write(sep);
                sep = ",";
                keyWriter.Reset();
                writer.Write("{");

                if (!string.IsNullOrWhiteSpace(rule.Rule.Name))
                {
                    keyWriter.Write("name:");
                    writer.Write(JavaScriptHelper.TranslateValue(rule.Rule.Name, options.CurrentCulture()));
                }

                if (!string.IsNullOrWhiteSpace(rule.Rule.Group))
                {
                    keyWriter.Write("group:");
                    writer.Write(JavaScriptHelper.TranslateValue(rule.Rule.Group, options.CurrentCulture()));
                }

                if (rule.Rule.IsAsync)
                {
                    keyWriter.Write("async:true");
                }

                if (rule.Rule.RelatedProperties?.Any() == true)
                {
                    keyWriter.Write("related:");
                    this.WriteArray(writer, rule.Rule.RelatedProperties.Values);
                }

                if (rule.Rule.Annotations?.Any() == true)
                {
                    keyWriter.Write("annotations:");
                    this.WriteObject(writer, rule.Rule.Annotations);
                }

                keyWriter.Write("validate:");
                writer.Write(rule.ClientCode);

                writer.Write("}");
            }

            writer.Write("]");
        };

        private class KeyWriter
        {
            private readonly TextWriter writer;
            private bool hasKey;

            public KeyWriter(TextWriter writer) => this.writer = writer;

            public void Reset() => this.hasKey = false;

            public void Write(string text)
            {
                if (this.hasKey)
                {
                    this.writer.Write(",");
                }

                this.writer.Write(text);
                this.hasKey = true;
            }
        }

        private void WriteArray(TextWriter writer, ICollection<string> items)
        {
            var sep = string.Empty;
            writer.Write("[");

            if (items != null)
            {
                foreach (var item in items)
                {
                    writer.Write(sep);
                    writer.Write(JavaScriptHelper.TranslateValue(item, options.CurrentCulture()) ?? this.options.JsonConversion(item));
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
                    writer.Write(JavaScriptHelper.TranslateValue(parameter.Value, options.CurrentCulture()) ?? this.options.JsonConversion(parameter.Value));
                    sep = ",";
                }
            }

            writer.Write("}");
        }
    }
}