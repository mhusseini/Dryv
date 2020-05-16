using System.Collections.Generic;
using System.Text;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvAsyncClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public string GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules)
        {
            var sb = new StringBuilder();

            sb.AppendLine("function(m, context) { return [");
            var sep = string.Empty;
            foreach (var rule in translatedRules)
            {
                sb.AppendLine(sep);
                sb.Append(rule.Value);
                sep = ", ";
            }
            sb.AppendLine(@"].reduce(function(promiseChain, currentTask){
                return promiseChain.then(function(error){
                    return typeof error === ""string"" && error ? error : currentTask(m ,context);
                });
            }, Promise.resolve());}");

            return sb.ToString();
        }
    }
}