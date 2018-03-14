using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dryv.Demo.Nav
{
    public class NavStructure
    {
        public MethodInfo Action { get; set; }
        public string Caption { get; set; }
        public List<NavStructure> Children { get; set; } = new List<NavStructure>();
        public Type Controller { get; set; }
        public string Name { get; set; }

        public string ParentName { get; set; }
        public NavStructure Parent { get; set; }
        public bool IsActive { get; internal set; }
    }
}