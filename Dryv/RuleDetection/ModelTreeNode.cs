using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Dryv.RuleDetection
{
    [DebuggerDisplay("{" + nameof(ModelPath) + "}")]
    public class ModelTreeNode
    {
        public string UniquePath { get; set; }

        public string ModelPath { get; set; }

        public Type ModelType { get; set; }

        public List<ModelTreeEdge> Children { get; set; } = new List<ModelTreeEdge>();

        public bool IsRecursive { get; set; }

        public List<MemberInfo> Hierarchy { get; set; }
    }
}