using System;
using System.Collections.Generic;
using System.IO;
using Dryv.RuleDetection;

namespace Dryv.Validation
{
    public interface IDryvClientValidationFunctionWriter
    {
        Action<Func<Type, object>, TextWriter> GetValidationFunction(IDictionary<DryvRuleTreeNode, Func<Func<Type, object>, string>> translatedRules);
    }
}