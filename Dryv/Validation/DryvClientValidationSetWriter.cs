using System;
using System.Collections.Generic;
using System.IO;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Translation;

namespace Dryv.Validation
{
    public class DryvClientValidationSetWriter : IDryvClientValidationSetWriter
    {
        private readonly DryvOptions options;

        public DryvClientValidationSetWriter(DryvOptions options)
        {
            this.options = options;
        }

        public virtual void WriteBegin(TextWriter writer)
        {
            writer.WriteLine("(function(dryv) { if (!dryv.v) { dryv.v = {}; }");
        }

        public virtual void WriteEnd(TextWriter writer)
        {
            writer.Write("})(window.dryv || (window.dryv = {}));");
        }

        public virtual void WriteValidationSet(TextWriter writer, string validationSetName, IDictionary<string, Action<TextWriter>> validators, IDictionary<string, Action<TextWriter>> disablers, Dictionary<string, object> parameters)
        {
            writer.Write(@"dryv.v[""");
            writer.Write(validationSetName);
            writer.Write(@"""] = { validators: ");
            WriteObject(writer, validators);
            writer.Write(", disablers: ");
            WriteObject(writer, disablers);
            writer.Write(", parameters: ");
            this.WriteParameters(writer, parameters);
            writer.Write("};");
        }

        private void WriteParameters(TextWriter writer, Dictionary<string, object> parameters)
        {
            var sep = string.Empty;
            writer.Write("{");

            foreach (var parameter in parameters)
            {
                writer.Write(sep);
                writer.Write("\"");
                writer.Write(parameter.Key.ToCamelCase());
                writer.Write("\":");
                writer.Write(JavaScriptHelper.TranslateValue(parameter.Value) ?? this.options.JsonConversion(parameter.Value));
                sep = ",";
            }

            writer.Write("}");
        }

        private static void WriteObject(TextWriter writer, IDictionary<string, Action<TextWriter>> items)
        {
            var sep = string.Empty;

            writer.Write("{");

            foreach (var item in items)
            {
                writer.Write(sep);
                writer.Write(@"""");
                writer.Write(item.Key);
                writer.Write(@""":");
                item.Value(writer);
                sep = ",";
            }

            writer.Write("}");
        }
    }
}