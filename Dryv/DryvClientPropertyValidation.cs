using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dryv
{
    public class DryvClientPropertyValidation
    {
        public string ValidationFunction { get; set; }
        public IDictionary<string, string> ElementAttribute { get; set; }
        public string ModelPath { get; set; }
        public Type ModelType { get; set; }
        public string Name { get; set; }
        public PropertyInfo Property { get; set; }
    }
}