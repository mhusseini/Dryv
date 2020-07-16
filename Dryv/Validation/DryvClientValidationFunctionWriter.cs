using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public class DryvClientValidationFunctionWriter : IDryvClientValidationFunctionWriter
    {
        public Action<Func<Type, object>, TextWriter> GetValidationFunction(IDictionary<DryvRuleTreeNode, Func<Func<Type, object>, string>> translatedRules) =>
            (serviceProvider, writer) =>
                writer.Write($@"function(m) {{ return {string.Join("||", translatedRules.Values.Select(f => $"({f(serviceProvider)}).call(this, m)"))}; }}");
    }
}