using System;
using System.Collections.Generic;
using System.IO;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvAsyncClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public Action<Func<Type, object>, TextWriter> GetValidationFunction(IDictionary<DryvRuleTreeNode, Func<Func<Type, object>, string>> translatedRules) =>
            (serviceProvider, writer) =>
            {
                writer.Write("dryv.r.bind(this, [");
                var sep = string.Empty;

                foreach (var rule in translatedRules)
                {
                    writer.Write(sep);
                    writer.Write(rule.Value(serviceProvider));
                    sep = ",";
                }

                writer.Write(@"])");
            };
    }
}