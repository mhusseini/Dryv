using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dryv.Validation
{
    public class DryvClientValidationNamedGroup
    {
        public string GroupName { get; set; }
        public string Key { get; set; }
        public string ModelPath { get; set; }
        public Type ModelType { get; set; }
        public IList<PropertyInfo> Properties { get; set; }
        public string ValidationFunction { get; set; }
    }
}