using System;
using System.Collections.Generic;

namespace Dryv.Rules
{
    public interface IDryvRuleTreeBuilder
    {
        IReadOnlyCollection<DryvRuleSet> Build(Type root);

        IReadOnlyCollection<DryvRuleSet> Build(Type root, string ruleSetName);

        IReadOnlyCollection<DryvRuleSet> Build(Type root, string ruleSetName, IEnumerable<IDryvRuleSource> sources);
    }
}