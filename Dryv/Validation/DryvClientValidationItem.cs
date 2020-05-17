using System;
using System.Reflection;

namespace Dryv.Validation
{
    public class DryvClientValidationItem
    {
        public string DisablingFunction { get; set; }
        public string GroupName { get; set; }
        public string Key { get; set; }
        public string ModelPath { get; set; }
        public Type ModelType { get; set; }
        public PropertyInfo Property { get; set; }
        public string ValidationFunction { get; set; }
    }
}