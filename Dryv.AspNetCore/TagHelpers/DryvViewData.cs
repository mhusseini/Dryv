using System.Collections.Generic;

namespace Dryv.AspNetCore.TagHelpers
{
    internal class DryvViewData
    {
        public IDictionary<string, string> ValidationFunctions { get; set; } = new Dictionary<string, string>();
    }
}