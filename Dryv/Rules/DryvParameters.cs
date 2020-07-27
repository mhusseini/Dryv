using System;
using System.Collections.Generic;

namespace Dryv.Rules
{
    public class DryvParameters
    {
        private readonly IDictionary<string, object> values;

        public DryvParameters(IDictionary<string, object> values)
        {
            this.values = values;
        }

        public T Get<T>(string name)
        {
            return this.values.TryGetValue(name, out var o) && o is T value
                ? value
                : default;
        }
    }
}