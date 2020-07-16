using System;
using System.Collections.Generic;
using System.IO;

namespace Dryv.Validation
{
    public class DryvClientValidationSetWriter : IDryvClientValidationSetWriter
    {
        public virtual void WriteBegin(TextWriter writer, Func<Type, object> serviceProvider)
        {
            writer.WriteLine("(function(dryv) { if (!dryv.v) { dryv.v = {}; }");
            writer.WriteLine("if (!dryv.r) { dryv.r = function(v, m, context) { return v.reduce(function(promiseChain, currentTask){ return promiseChain.then(function(r){ return r||currentTask(m, context); }); }, Promise.resolve());} }");
        }

        public virtual void WriteEnd(TextWriter writer, Func<Type, object> serviceProvider)
        {
            writer.Write("})(window.dryv || (window.dryv = {}));");
        }

        public virtual void WriteValidationSet(TextWriter writer, string validationSetName, IDictionary<string, Action<Func<Type, object>, TextWriter>> validators, IDictionary<string, Action<Func<Type, object>, TextWriter>> disablers, Func<Type, object> servicePRovider)
        {
            writer.Write(@"dryv.v[""");
            writer.Write(validationSetName);
            writer.Write(@"""] = { validators: ");
            WriteObject(writer, validators, servicePRovider);
            writer.Write(", disablers: ");
            WriteObject(writer, disablers, servicePRovider);
            writer.Write("};");
        }

        private static void WriteObject(TextWriter writer, IDictionary<string, Action<Func<Type, object>, TextWriter>> items, Func<Type, object> servicePRovider)
        {
            var sep = string.Empty;

            writer.Write("{");

            foreach (var item in items)
            {
                writer.Write(sep);
                writer.Write(@"""");
                writer.Write(item.Key);
                writer.Write(@""":");
                item.Value(servicePRovider, writer);
                sep = ",";
            }

            writer.Write("}");
        }
    }
}