using System.Collections.Generic;
using System.Text;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvAsyncAwaitClientValidationProvider : DryvClientValidationProvider
    {
        protected override string GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules)
        {
            var sb = new StringBuilder();

            sb.AppendLine("async function(m, context) {");
            sb.AppendLine("let error;");

            foreach (var rule in translatedRules)
            {
                sb.AppendLine("error = " + rule.Value + "(m, context);");
                sb.AppendLine("if (error) return error;");
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}