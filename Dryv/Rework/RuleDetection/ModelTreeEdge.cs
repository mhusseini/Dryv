using System.Diagnostics;
using System.Reflection;

namespace Dryv.Rework.RuleDetection
{
    [DebuggerDisplay("{Property.Name}")]
    internal class ModelTreeEdge
    {
        public PropertyInfo Property { get; set; }

        public ModelTreeNode Parent { get; set; }

        public ModelTreeNode Child { get; set; }
    }
}