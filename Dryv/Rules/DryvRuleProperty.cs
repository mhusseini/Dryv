using System.Collections.Generic;
using System.Reflection;

namespace Dryv.Rules
{
    public class DryvRuleProperty
    {
        public PropertyInfo Property { get; set; }

        public string PropertyPath { get; set; }

        public List<MemberInfo> PropertyHierarchy { get; set; }

        public bool IsGlobal { get; set; }
    }
}