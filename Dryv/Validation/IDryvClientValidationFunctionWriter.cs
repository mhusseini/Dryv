using System;
using System.Collections.Generic;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public interface IDryvClientValidationFunctionWriter
    {
        string GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules);
    }
}