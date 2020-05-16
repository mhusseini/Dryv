using System.Collections.Generic;
using System.Linq;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public string GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules)
        {
            return $@"function(m) {{ return {string.Join("||", translatedRules.Values.Select(f => $"({f}).call(this, m)"))}; }}";
        }
    }
}