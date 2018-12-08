using System;
using System.Collections.Generic;
using System.Reflection;
using Dryv.Utils;

namespace Dryv
{
    public class DryvVeeValidateModelValidator : DryvClientModelValidator
    {
        protected override IDictionary<string, string> CreateValidationAttributes(Type modelType, PropertyInfo property, string name)
        {
            return new Dictionary<string, string>
            {
                { "v-validate", $"'dryv:{name}'" },
                { "dryv-v", name },
                { "dryv-t", property.PropertyType.GetJavaScriptType() }
            };
        }
    }
}