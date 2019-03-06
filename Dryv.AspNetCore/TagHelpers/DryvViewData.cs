using System.Collections.Generic;
using Dryv.Validation;

namespace Dryv.TagHelpers
{
    internal class DryvViewData
    {
        public IDictionary<string, DryvClientValidationItem> ValidationFunctions { get; set; } = new Dictionary<string, DryvClientValidationItem>();
    }
}