using System;
using System.Collections.Generic;
using System.IO;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public interface IDryvClientValidationFunctionWriter
    {
        Action<TextWriter> GetValidationFunction(IDictionary<DryvRuleTreeNode, Func<string>> translatedRules);
    }
}