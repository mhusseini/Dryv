using System.Collections.Generic;
using System.Text;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvAsyncClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public string GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules)
        {
            var sb = new StringBuilder("function(m, context) { return [");
            
            var index = 0;
            
            foreach (var rule in translatedRules)
            {
                sb.Append(rule.Value);
                if (++index > 0)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine(@"].reduce(function(promiseChain, currentTask){ return promiseChain.then(function(error){ return error || currentTask(m ,context); }); }, Promise.resolve());}");

            return sb.ToString();
        }
    }
}