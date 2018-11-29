using System.Collections.Generic;

namespace Dryv
{
    public class DryvTagHelperResult
    {
        public IDictionary<string, string> ValidationFunctions { get; set; } = new Dictionary<string, string>();
    }
}