using System;

namespace Dryv.AspNetCore.DynamicControllers.Runtime
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class DryvControllerInfoAttribute : Attribute
    {
        public string Name { get; }
        public string Value { get; }

        public DryvControllerInfoAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}