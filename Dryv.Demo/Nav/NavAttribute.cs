using System;

namespace Dryv.Demo
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NavAttribute : Attribute
    {
        public NavAttribute(string caption)
        {
            Caption = caption;
            Name = caption;
        }

        public NavAttribute(string caption, string name) : this(caption)
        {
            Name = name;
        }

        public string Caption { get; }

        public string Name { get; set; }
        public string Parent { get; set; }
    }
}