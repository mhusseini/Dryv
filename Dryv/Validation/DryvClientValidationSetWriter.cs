using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dryv.Validation
{
    public class DryvClientValidationSetWriter : IDryvClientValidationSetWriter
    {
        public virtual void WriteBegin(TextWriter writer)
        {
            writer.WriteLine("(function(dryv) { if (!dryv.v) { dryv.v = {}; }");
            writer.WriteLine("if (!dryv.r) { dryv.r = function(v, m, context) { return v.reduce(function(promiseChain, currentTask){ return promiseChain.then(function(r){ return r||currentTask(m, context); }); }, Promise.resolve());} }");
        }

        public virtual void WriteEnd(TextWriter writer)
        {
            writer.Write("})(window.dryv || (window.dryv = {}));");
        }

        public virtual void WriteValidationSet(TextWriter writer, string validationSetName, IDictionary<string, Action<TextWriter>> validators, IDictionary<string, Action<TextWriter>> disablers)
        {
            writer.Write(@"dryv.v[""");
            writer.Write(validationSetName);
            writer.Write(@"""] = { validators: ");
            WriteObject(writer, validators);
            writer.Write(", disablers: ");
            WriteObject(writer, disablers);
            writer.Write("};");
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