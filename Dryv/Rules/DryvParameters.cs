using System;
using System.Collections.Generic;

namespace Dryv.Rules
{
    public class DryvParameters
    {
        public DryvParameters(IDictionary<string, object> values)
        {
            this.Values = values;
        }

        public IDictionary<string, object> Values { get; }

        public T Get<T>(string name)
        {
            return this.Values.TryGetValue(name, out var o) && o is T value
                ? value
                : default;
        }
    }
}