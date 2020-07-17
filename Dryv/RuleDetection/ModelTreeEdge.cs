using System.Diagnostics;
using System.Reflection;

namespace Dryv.RuleDetection
{
    [DebuggerDisplay("{Property.Name}")]
    public class ModelTreeEdge
    {
        public PropertyInfo Property { get; set; }

        public ModelTreeNode Parent { get; set; }

        public ModelTreeNode Child { get; set; }
    }
}