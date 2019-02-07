using System.Collections.Generic;

namespace Dryv.TagHelpers
{
    internal class DryvViewData
    {
        public IDictionary<string, DryvClientPropertyValidation> ValidationFunctions { get; set; } = new Dictionary<string, DryvClientPropertyValidation>();
    }
}