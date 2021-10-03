using System;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DryvSetAttribute : Attribute
    {
        public string Name { get; }

        public DryvSetAttribute(string name)
        {
            Name = name;
        }
    }
}