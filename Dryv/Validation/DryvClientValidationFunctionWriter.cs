using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public Action<TextWriter> GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules)
        {
            return writer => writer.Write($@"function(m) {{ return {string.Join("||", translatedRules.Values.Select(f => $"({f}).call(this, m)"))}; }}");
        }
    }
}