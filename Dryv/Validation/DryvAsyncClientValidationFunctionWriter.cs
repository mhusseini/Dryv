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
                var index = 0;

                foreach (var rule in translatedRules)
                {
                    writer.Write(rule.Value);
                    if (++index > 0)
                    {
                        writer.Write(",");
                    }
                }

                writer.Write(@"].reduce(function(promiseChain, currentTask){ return promiseChain.then(function(error){ return error || currentTask(m, context); }); }, Promise.resolve());}");
            };
    }
}