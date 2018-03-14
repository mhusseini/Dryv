using System;

namespace Dryv.Demo.Nav
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NavAttribute : Attribute
    {
        public NavAttribute(string caption)
        {
            this.Caption = caption;
        }

        public string After { get; set; }

        public string Caption { get; }

        public string Parent { get; set; }
    }
}