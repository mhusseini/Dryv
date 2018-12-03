using System.Collections.Generic;

namespace Dryv.TagHelpers
{
    internal class DryvViewData
    {
        public IDictionary<string, string> ValidationFunctions { get; set; } = new Dictionary<string, string>();
    }
}