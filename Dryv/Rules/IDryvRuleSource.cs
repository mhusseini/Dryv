using System;
using System.Collections.Generic;

namespace Dryv.Rules
{
    public interface IDryvRuleSource
    {
        IReadOnlyCollection<DryvRule> GetRules(Type root);

        IReadOnlyCollection<DryvRule> GetRules(Type root, string ruleSetName);
    }
}