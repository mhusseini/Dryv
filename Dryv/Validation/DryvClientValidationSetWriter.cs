using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dryv.Validation
{
    public class DryvClientValidationSetWriter : IDryvClientValidationSetWriter
    {
        public virtual void WriteBegin(TextWriter writer)
        {
            writer.Write("(function(dryv, viewName) { if (!dryv.v) { dryv.v = {}; }");
        }

        public virtual void WriteEnd(TextWriter writer)
        {
            writer.Write("})(window.dryv || (window.dryv = {}));");
        }

        public virtual void WriteValidationSet(TextWriter writer, string validationSetName, IDictionary<string, string> validators, IDictionary<string, string> disablers)
        {
            writer.Write("dryv.v[viewName] = { validators: {");
            writer.Write(WriteObject(validators));
            writer.Write("}, disablers: {");
            writer.Write(WriteObject(disablers));
            writer.Write("}};");
        }

        private static string WriteObject(IDictionary<string, string> items)
        {
            return string.Join(",\n", items.Select(val => $@"""{val.Key}"": {val.Value}"));
        }
    }
}