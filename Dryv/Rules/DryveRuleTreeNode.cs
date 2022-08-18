using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Dryv.Rules
{
    [DebuggerDisplay("{Property.DeclaringType.Name}.{Property.Name}")]
    public class DryveRuleTreeNode
    {
        public PropertyInfo Property { get; set; }

        public List<DryvRule> Rules { get; set; }

        public List<DryveRuleTreeNode> Children { get; set; }
    }
}