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
                writer.Write("function(m, context) { return [");
                var sep = string.Empty;

                foreach (var rule in translatedRules)
                {
                    writer.Write(sep);
                    writer.Write(rule.Value);
                    sep = ",";
                }

                writer.Write(@"].reduce(function(promiseChain, currentTask){ return promiseChain.then(function(r){ return r||currentTask(m, context); }); }, Promise.resolve());}");
            };
    }
}