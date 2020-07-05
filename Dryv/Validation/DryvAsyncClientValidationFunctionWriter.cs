using System;
using System.Collections.Generic;
using System.IO;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvAsyncClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public Action<TextWriter> GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules) => writer =>
            {
                writer.Write("dryv.r.bind(this, [");
                var sep = string.Empty;

                foreach (var rule in translatedRules)
                {
                    writer.Write(sep);
                    writer.Write(rule.Value);
                    sep = ",";
                }

                writer.Write(@"])");
            };
    }
}